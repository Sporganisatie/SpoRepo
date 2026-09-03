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

        var accountParticipations = DB.AccountParticipations.AsNoTracking()
            .Include(ap => ap.RiderParticipations)
            .Where(ap => allParticipations.Select(rp => rp.RaceId).Contains(ap.RaceId) && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        var totalParticipantsByRace = accountParticipations
            .GroupBy(ap => ap.RaceId)
            .ToDictionary(g => g.Key, g => g.Count());

        var selectedCountsByParticipation = accountParticipations
            .SelectMany(ap => ap.RiderParticipations)
            .GroupBy(rp => rp.RiderParticipationId)
            .ToDictionary(g => g.Key, g => g.Count());

        var participations = allParticipations
            .Where(rp => totalParticipantsByRace.GetValueOrDefault(rp.RaceId) > 0)
            .ToList();

        var finalStandingsStages = DB.Stages.AsNoTracking()
            .Where(s => s.Type == StageType.FinalStandings && participations.Select(rp => rp.RaceId).Contains(s.RaceId))
            .ToDictionary(s => s.RaceId);

        var allResults = DB.ResultsPoints.AsNoTracking()
            .Where(rp => participations.Select(p => p.RiderParticipationId).Contains(rp.RiderParticipationId))
            .ToList();

        var finalResults = allResults
            .Where(rp => finalStandingsStages.Values.Select(s => s.StageId).Contains(rp.StageId))
            .ToDictionary(rp => rp.StageId);

        var totalScoresByParticipation = allResults
            .GroupBy(rp => rp.RiderParticipationId)
            .ToDictionary(g => g.Key, g => g.Sum(rp => AdjustedScore(rp, budgetParticipation)));

        var races = participations
            .OrderByDescending(rp => rp.Race.Year)
            .ThenByDescending(rp => rp.Race.Name)
            .Select(rp =>
            {
                finalStandingsStages.TryGetValue(rp.RaceId, out var finalStandingsStage);
                var result = finalStandingsStage is null ? null : finalResults.GetValueOrDefault(finalStandingsStage.StageId);

                return new RiderRaceOverviewRow(
                    rp.RaceId,
                    Capitalize(rp.Race.Name),
                    rp.Race.Year,
                    !rp.Race.Finished,
                    rp.Dnf,
                    rp.Price,
                    IsTooExpensive(rp.Price, budgetParticipation),
                    totalScoresByParticipation.GetValueOrDefault(rp.RiderParticipationId),
                    totalParticipantsByRace.GetValueOrDefault(rp.RaceId),
                    selectedCountsByParticipation.GetValueOrDefault(rp.RiderParticipationId),
                    Position(result?.Gc),
                    Position(result?.Points),
                    Position(result?.Kom),
                    Position(result?.Youth));
            }).ToList();

        return new RiderOverview(
            rider.RiderId,
            rider.Firstname,
            rider.Lastname,
            races,
            GetSelectionHistory(participations, budgetParticipation));
    }

    private List<RiderSelectionHistoryRow> GetSelectionHistory(List<RiderParticipation> participations, bool budgetParticipation)
    {
        var eligibleParticipations = participations
            .Where(rp => !IsTooExpensive(rp.Price, budgetParticipation))
            .ToList();
        var eligibleRaceIds = eligibleParticipations.Select(rp => rp.RaceId).ToList();
        var eligibleRiderParticipationIds = eligibleParticipations.Select(rp => rp.RiderParticipationId).ToHashSet();

        var accountParticipations = DB.AccountParticipations.AsNoTracking()
            .Include(ap => ap.Account)
            .Include(ap => ap.RiderParticipations)
            .Where(ap => eligibleRaceIds.Contains(ap.RaceId) && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        return accountParticipations
            .GroupBy(ap => ap.Account.Username)
            .Select(g =>
            {
                var selectedRaceIds = g
                    .Where(ap => ap.RiderParticipations.Any(rp => eligibleRiderParticipationIds.Contains(rp.RiderParticipationId)))
                    .Select(ap => ap.RaceId)
                    .ToList();
                var races = eligibleParticipations
                    .Where(rp => selectedRaceIds.Contains(rp.RaceId))
                    .OrderByDescending(rp => rp.Race.Year)
                    .ThenByDescending(rp => rp.Race.Name)
                    .Select(rp => $"{Capitalize(rp.Race.Name)} {rp.Race.Year}")
                    .ToList();
                return new RiderSelectionHistoryRow(g.Key, selectedRaceIds.Count, g.Select(ap => ap.RaceId).Distinct().Count(), races);
            })
            .OrderByDescending(r => r.SelectedCount)
            .ThenBy(r => r.Username)
            .ToList();
    }

    internal RiderRaceDetail GetRiderRaceDetail(int riderId, int raceId, bool budgetParticipation)
    {
        var participation = DB.RiderParticipations.AsNoTracking()
            .Include(rp => rp.Rider)
            .Include(rp => rp.Race)
            .Single(rp => rp.RaceId == raceId && rp.RiderId == riderId);

        var stages = DB.Stages.AsNoTracking()
            .Where(s => s.RaceId == raceId)
            .OrderBy(s => s.Stagenr)
            .ToList();

        var active = !participation.Race.Finished;

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
            .Include(ap => ap.RiderParticipations)
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        var loggedInParticipationId = accountParticipations.SingleOrDefault(ap => ap.AccountId == User.Id)?.AccountParticipationId;

        var teamOwners = accountParticipations
            .Where(ap => ap.RiderParticipations.Any(rp => rp.RiderParticipationId == participation.RiderParticipationId))
            .ToList();

        var stageSelectionsByRider = DB.StageSelections.AsNoTracking()
            .Where(ss => ss.AccountParticipation.RaceId == raceId
                && ss.AccountParticipation.BudgetParticipation == budgetParticipation
                && ss.Stage.Type != StageType.FinalStandings
                && ss.RiderParticipations.Any(rp => rp.RiderParticipationId == participation.RiderParticipationId))
            .ToList();

        var selections = teamOwners
            .Select(ap =>
            {
                var stageSelections = stageSelectionsByRider
                    .Where(ss => ss.AccountParticipationId == ap.AccountParticipationId)
                    .ToList();
                var points = stageSelections.Sum(ss =>
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
                    ap.Account.Username,
                    points,
                    stageSelections.Count,
                    stageSelections.Count(ss => ss.KopmanId == participation.RiderParticipationId),
                    ap.AccountParticipationId == loggedInParticipationId);
            })
            .OrderByDescending(r => r.Points)
            .ThenBy(r => r.Username)
            .ToList();

        return new RiderRaceDetail(
            participation.Rider.Firstname,
            participation.Rider.Lastname,
            IsTooExpensive(participation.Price, budgetParticipation),
            active,
            participation.Dnf,
            participation.Price,
            accountParticipations.Count,
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

    private static string Capitalize(string value)
        => string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];
}
