using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class RiderService(DatabaseContext DB, Userdata User)
{
    private const int BudgetCapMaxRiderPrice = 750_000;

    private static bool IsTooExpensive(int price, bool budgetParticipation)
        => budgetParticipation && price > BudgetCapMaxRiderPrice;

    private static int AdjustedScore(ResultsPoint rp, bool budgetParticipation)
        => rp.Totalscore - (budgetParticipation ? rp.Teamscore : 0);

    internal RiderOverview GetRiderOverview(int riderId, bool budgetParticipation)
    {
        var rider = DB.Riders.AsNoTracking().Single(r => r.RiderId == riderId);

        var allParticipations = DB.RiderParticipations.AsNoTracking()
            .Include(rp => rp.Race)
            .Where(rp => rp.RiderId == riderId && rp.RaceId != 99 && rp.Race.Name != "classics")
            .ToList();

        var allRaceIds = allParticipations.Select(rp => rp.RaceId).ToList();
        var totalParticipantsByRace = DB.AccountParticipations.AsNoTracking()
            .Where(ap => allRaceIds.Contains(ap.RaceId) && ap.BudgetParticipation == budgetParticipation)
            .GroupBy(ap => ap.RaceId)
            .ToDictionary(g => g.Key, g => g.Count());

        // Races with no tracked participants (old, untracked seasons or races nobody has joined yet) have no
        // race-specific data to show, so they're excluded entirely rather than shown with blank/zeroed columns.
        var participations = allParticipations
            .Where(rp => totalParticipantsByRace.GetValueOrDefault(rp.RaceId) > 0)
            .ToList();

        var raceIds = participations.Select(rp => rp.RaceId).ToList();
        var finalStandingsStages = DB.Stages.AsNoTracking()
            .Where(s => raceIds.Contains(s.RaceId) && s.Type == StageType.FinalStandings)
            .ToDictionary(s => s.RaceId);

        var finalStandingsStageIds = finalStandingsStages.Values.Select(s => s.StageId).ToList();
        var riderParticipationIds = participations.Select(rp => rp.RiderParticipationId).ToList();
        var finalResults = DB.ResultsPoints.AsNoTracking()
            .Where(rp => finalStandingsStageIds.Contains(rp.StageId) && riderParticipationIds.Contains(rp.RiderParticipationId))
            .ToDictionary(rp => rp.StageId);

        var totalScoresByParticipation = DB.ResultsPoints.AsNoTracking()
            .Where(rp => riderParticipationIds.Contains(rp.RiderParticipationId))
            .ToList()
            .GroupBy(rp => rp.RiderParticipationId)
            .ToDictionary(g => g.Key, g => g.Sum(rp => AdjustedScore(rp, budgetParticipation)));

        var races = participations
            .OrderByDescending(rp => rp.Race.Year)
            .ThenByDescending(rp => rp.Race.Name)
            .Select(rp =>
            {
                finalStandingsStages.TryGetValue(rp.RaceId, out var finalStandingsStage);
                var result = finalStandingsStage is null ? null : finalResults.GetValueOrDefault(finalStandingsStage.StageId);

                var active = finalStandingsStage is not null && !finalStandingsStage.Finished;
                var totalParticipants = totalParticipantsByRace.GetValueOrDefault(rp.RaceId);
                var selectedCount = SelectedByCount(rp.RaceId, rp.RiderParticipationId, budgetParticipation);

                return new RiderRaceOverviewRow(
                    rp.RaceId,
                    Capitalize(rp.Race.Name),
                    rp.Race.Year,
                    active,
                    rp.Dnf,
                    rp.Price,
                    IsTooExpensive(rp.Price, budgetParticipation),
                    totalScoresByParticipation.GetValueOrDefault(rp.RiderParticipationId),
                    totalParticipants,
                    selectedCount,
                    Position(result?.Gc),
                    Position(result?.Points),
                    Position(result?.Kom),
                    Position(result?.Youth));
            }).ToList();

        var selectionHistory = GetSelectionHistory(participations, budgetParticipation);

        return new RiderOverview(rider.RiderId, rider.Firstname, rider.Lastname, races, selectionHistory);
    }

    private List<RiderSelectionHistoryRow> GetSelectionHistory(
        List<RiderParticipation> participations,
        bool budgetParticipation)
    {
        var eligibleParticipations = participations
            .Where(rp => !IsTooExpensive(rp.Price, budgetParticipation))
            .ToList();
        var eligibleRaceIds = eligibleParticipations.Select(rp => rp.RaceId).ToList();
        var eligibleRiderParticipationIds = eligibleParticipations.Select(rp => rp.RiderParticipationId).ToList();

        var mutualByUsername = DB.AccountParticipations.AsNoTracking()
            .Include(ap => ap.Account)
            .Where(ap => eligibleRaceIds.Contains(ap.RaceId) && ap.BudgetParticipation == budgetParticipation)
            .ToList()
            .GroupBy(ap => ap.Account.Username)
            .ToDictionary(g => g.Key, g => g.Select(ap => ap.RaceId).Distinct().ToList());

        var selectedRacesByUsername = DB.StageSelections.AsNoTracking()
            .Include(ss => ss.RiderParticipations)
            .Include(ss => ss.Stage)
            .Include(ss => ss.AccountParticipation).ThenInclude(ap => ap.Account)
            .Where(ss => eligibleRaceIds.Contains(ss.AccountParticipation.RaceId)
                && ss.AccountParticipation.BudgetParticipation == budgetParticipation
                && ss.Stage.Type != StageType.FinalStandings
                && ss.RiderParticipations.Any(rp => eligibleRiderParticipationIds.Contains(rp.RiderParticipationId)))
            .Select(ss => new { ss.AccountParticipation.RaceId, ss.AccountParticipation.Account.Username })
            .Distinct()
            .ToList()
            .GroupBy(x => x.Username)
            .ToDictionary(g => g.Key, g => g.Select(x => x.RaceId).ToList());

        return mutualByUsername
            .Select(kvp =>
            {
                var username = kvp.Key;
                var totalRaceIds = kvp.Value;
                var selectedRaceIds = selectedRacesByUsername.GetValueOrDefault(username, []);
                var races = eligibleParticipations
                    .Where(rp => selectedRaceIds.Contains(rp.RaceId))
                    .OrderByDescending(rp => rp.Race.Year)
                    .ThenByDescending(rp => rp.Race.Name)
                    .Select(rp => $"{Capitalize(rp.Race.Name)} {rp.Race.Year}")
                    .ToList();
                return new RiderSelectionHistoryRow(username, selectedRaceIds.Count, totalRaceIds.Count, races);
            })
            .OrderByDescending(r => r.SelectedCount)
            .ThenBy(r => r.Username)
            .ToList();
    }

    internal RiderRaceDetail GetRiderRaceDetail(int riderId, int raceId, bool budgetParticipation)
    {
        var participation = DB.RiderParticipations.AsNoTracking()
            .Include(rp => rp.Rider)
            .Single(rp => rp.RaceId == raceId && rp.RiderId == riderId);

        var stages = DB.Stages.AsNoTracking()
            .Where(s => s.RaceId == raceId)
            .OrderBy(s => s.Stagenr)
            .ToList();

        var finalStandingsStage = stages.SingleOrDefault(s => s.Type == StageType.FinalStandings);
        var active = finalStandingsStage is not null && !finalStandingsStage.Finished;

        var results = DB.ResultsPoints.AsNoTracking()
            .Where(rp => rp.RiderParticipationId == participation.RiderParticipationId)
            .ToDictionary(rp => rp.StageId);

        IEnumerable<Stage> shownStages = stages;
        if (active)
        {
            shownStages = shownStages.Where(s => s.Finished);
        }
        shownStages = shownStages.Where(s => results.ContainsKey(s.StageId)).ToList();

        var stageRows = shownStages.Select(s =>
        {
            var rp = results[s.StageId];
            return new RiderStageScoreRow(
                s.Stagenr,
                StageLabel(s, "Eindpunten"),
                rp.StageScore,
                rp.Gc.Score ?? 0,
                rp.Points.Score ?? 0,
                rp.Kom.Score ?? 0,
                rp.Youth.Score ?? 0,
                budgetParticipation ? 0 : rp.Teamscore,
                AdjustedScore(rp, budgetParticipation));
        }).ToList();

        stageRows.Add(new RiderStageScoreRow(
            null,
            "Totaal",
            stageRows.Sum(r => r.StageScore ?? 0),
            stageRows.Sum(r => r.GcScore),
            stageRows.Sum(r => r.PointsScore),
            stageRows.Sum(r => r.KomScore),
            stageRows.Sum(r => r.YouthScore),
            stageRows.Sum(r => r.TeamScore),
            stageRows.Sum(r => r.TotalScore)));

        var classifications = shownStages.Select(s =>
        {
            var rp = results[s.StageId];
            return new RiderClassificationRow(
                s.Stagenr,
                StageLabel(s, "Eindstand"),
                Position(rp.StagePos),
                rp.StageResult ?? "",
                Cell(rp.Gc),
                Cell(rp.Points),
                Cell(rp.Kom),
                Cell(rp.Youth));
        }).ToList();

        var accountParticipations = DB.AccountParticipations.AsNoTracking()
            .Include(ap => ap.Account)
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        var loggedInParticipationId = accountParticipations.SingleOrDefault(ap => ap.AccountId == User.Id)?.AccountParticipationId;

        var stageSelections = DB.StageSelections.AsNoTracking()
            .Include(ss => ss.RiderParticipations)
            .Include(ss => ss.Stage)
            .Where(ss => ss.AccountParticipation.RaceId == raceId
                && ss.AccountParticipation.BudgetParticipation == budgetParticipation
                && ss.Stage.Type != StageType.FinalStandings
                && ss.RiderParticipations.Any(rp => rp.RiderParticipationId == participation.RiderParticipationId))
            .ToList();

        var selections = stageSelections
            .GroupBy(ss => ss.AccountParticipationId)
            .Select(g =>
            {
                var account = accountParticipations.Single(ap => ap.AccountParticipationId == g.Key);
                var points = g.Sum(ss =>
                {
                    var isKopman = ss.KopmanId == participation.RiderParticipationId;
                    results.TryGetValue(ss.StageId, out var rp);
                    if (rp is null)
                    {
                        return 0;
                    }
                    return (int)((isKopman ? rp.StageScore * 0.5 : 0) + AdjustedScore(rp, budgetParticipation));
                });
                return new RiderSelectionRow(
                    account.Account.Username,
                    points,
                    g.Count(),
                    g.Count(ss => ss.KopmanId == participation.RiderParticipationId),
                    account.AccountParticipationId == loggedInParticipationId);
            })
            .OrderByDescending(r => r.Points)
            .ToList();

        return new RiderRaceDetail(
            participation.Rider.Firstname,
            participation.Rider.Lastname,
            IsTooExpensive(participation.Price, budgetParticipation),
            active,
            participation.Dnf,
            participation.Price,
            accountParticipations.Count,
            selections.Count,
            stageRows,
            classifications,
            selections);
    }

    private static int? Position(BaseResult result)
        => Position(result?.Position);

    private static int? Position(int? position)
        => position is null or <= 0 ? null : position;

    private static ClassificationCell Cell(BaseResult result)
        => new(Position(result), result?.Result ?? "");

    private static string StageLabel(Stage stage, string finalStandingsLabel)
        => stage.Type == StageType.FinalStandings ? finalStandingsLabel : stage.Stagenr.ToString();

    private int SelectedByCount(int raceId, int riderParticipationId, bool budgetParticipation)
        => DB.StageSelections.AsNoTracking()
            .Where(ss => ss.AccountParticipation.RaceId == raceId
                && ss.AccountParticipation.BudgetParticipation == budgetParticipation
                && ss.Stage.Type != StageType.FinalStandings
                && ss.RiderParticipations.Any(rp => rp.RiderParticipationId == riderParticipationId))
            .Select(ss => ss.AccountParticipationId)
            .Distinct()
            .Count();

    private static string Capitalize(string value)
        => string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];
}
