using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public record RacePayload(
    int RaceId,
    string Name,
    int Year,
    List<RacePayloadPlayer> Players
);

public record RacePayloadPlayer(
    string Username,
    int FinalScore,
    int FinalRank,
    int KopmanBonus,
    double KopmanPctOfTotal,
    int StageWins,
    int RedLanterns,
    int TotalGemist,
    int TotalOptimal,
    List<MissedPointsData> MissedPointsPerStage,
    int DnfRiderCount,
    int DnfBudgetLost,
    int UitvallerStagesBudget,
    int MaxStageScore,
    int MinStageScore,
    Dictionary<int, int> CumulativeAtStage
);

public class RaceStatsService(DatabaseContext DB, StatisticsService Statistics)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public RacePayload GetRacePayload(int raceId, bool budgetParticipation)
    {
        var existing = DB.RaceStats.AsNoTracking()
            .FirstOrDefault(r => r.RaceId == raceId && r.BudgetParticipation == budgetParticipation);
        if (existing != null)
        {
            return JsonSerializer.Deserialize<RacePayload>(existing.Payload, JsonOpts)!;
        }

        var built = BuildRacePayload(raceId, budgetParticipation);
        SaveRacePayload(raceId, budgetParticipation, built);
        return built;
    }

    public List<RacePayload> LoadAllRacePayloads(bool budgetParticipation)
    {
        var raceIds = DB.Races
            .Where(r => r.RaceId != 99 && r.Name != "classics")
            .Select(r => r.RaceId)
            .ToList();

        var cached = DB.RaceStats.AsNoTracking()
            .Where(r => r.BudgetParticipation == budgetParticipation && raceIds.Contains(r.RaceId))
            .ToList()
            .ToDictionary(r => r.RaceId, r => JsonSerializer.Deserialize<RacePayload>(r.Payload, JsonOpts)!);

        var result = new List<RacePayload>(raceIds.Count);
        foreach (var id in raceIds)
        {
            if (cached.TryGetValue(id, out var p)) result.Add(p);
            else
            {
                var built = BuildRacePayload(id, budgetParticipation);
                SaveRacePayload(id, budgetParticipation, built);
                result.Add(built);
            }
        }
        return result;
    }

    public List<StageStats> LoadStageStats(bool budgetParticipation)
    {
        EnsureStageStats(budgetParticipation);
        return DB.StageStats.AsNoTracking()
            .Where(s => s.BudgetParticipation == budgetParticipation)
            .ToList();
    }

    public void InvalidateRace(int raceId)
    {
        DB.StageStats.Where(s => s.RaceId == raceId).ExecuteDelete();
        DB.RaceStats.Where(r => r.RaceId == raceId).ExecuteDelete();
    }

    public void InvalidateStage(int raceId) => InvalidateRace(raceId);

    public void InvalidateAll()
    {
        DB.StageStats.ExecuteDelete();
        DB.RaceStats.ExecuteDelete();
    }

    public (int RaceRows, int StageRows) WarmCache()
    {
        var raceBefore = DB.RaceStats.Count();
        _ = LoadAllRacePayloads(budgetParticipation: false);
        _ = LoadAllRacePayloads(budgetParticipation: true);
        var raceAfter = DB.RaceStats.Count();

        var stageBefore = DB.StageStats.Count();
        EnsureStageStats(budgetParticipation: false);
        EnsureStageStats(budgetParticipation: true);
        var stageAfter = DB.StageStats.Count();

        return (raceAfter - raceBefore, stageAfter - stageBefore);
    }

    private void EnsureStageStats(bool budgetParticipation)
    {
        var raceIds = DB.Races
            .Where(r => r.RaceId != 99 && r.Name != "classics")
            .Select(r => r.RaceId)
            .ToList();

        var presentRaceIds = DB.StageStats.AsNoTracking()
            .Where(s => s.BudgetParticipation == budgetParticipation)
            .Select(s => s.RaceId)
            .Distinct()
            .ToHashSet();

        foreach (var id in raceIds)
        {
            if (!presentRaceIds.Contains(id))
            {
                BuildStageStatsForRace(id, budgetParticipation);
            }
        }
    }

    public int BuildStageStatsForRace(int raceId, bool budgetParticipation)
    {
        DB.StageStats.Where(s => s.RaceId == raceId && s.BudgetParticipation == budgetParticipation).ExecuteDelete();

        var participantUsernames = DB.AccountParticipations.AsNoTracking()
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .Select(ap => new { ap.AccountParticipationId, Username = ap.Account.Username })
            .ToList()
            .ToDictionary(p => p.AccountParticipationId, p => p.Username);

        if (participantUsernames.Count == 0) return 0;

        var selections = DB.StageSelections.AsNoTracking()
            .Where(ss => ss.Stage.RaceId == raceId
                      && ss.Stage.Finished
                      && ss.Stage.Type != StageType.FinalStandings
                      && ss.AccountParticipation.BudgetParticipation == budgetParticipation)
            .Select(ss => new
            {
                ss.AccountParticipationId,
                Stagenr = ss.Stage.Stagenr,
                ss.StageId,
                ss.StageScore,
                ss.TotalScore,
                Picks = ss.RiderParticipations.Select(rp => new
                {
                    rp.RiderParticipationId,
                    rp.Price,
                }).ToList(),
            })
            .ToList();

        if (selections.Count == 0) return 0;

        var resultsByRiderStage = DB.ResultsPoints.AsNoTracking()
            .Where(rp => rp.Stage.RaceId == raceId)
            .Select(rp => new { rp.RiderParticipationId, rp.StageId })
            .ToList()
            .Select(x => (x.RiderParticipationId, x.StageId))
            .ToHashSet();

        var missedByUserStage = Statistics.MissedPoints(raceId, budgetParticipation)
            .ToDictionary(
                m => m.Username,
                m => m.Data
                    .Where(d => d.Etappe != "Totaal" && int.TryParse(d.Etappe, out _))
                    .ToDictionary(d => int.Parse(d.Etappe), d => d.Gemist));

        var inserted = 0;
        var now = DateTimeOffset.UtcNow;
        foreach (var ss in selections)
        {
            if (!participantUsernames.TryGetValue(ss.AccountParticipationId, out var username)) continue;

            var absentPicks = ss.Picks
                .Where(p => !resultsByRiderStage.Contains((p.RiderParticipationId, ss.StageId)))
                .ToList();

            var missed = missedByUserStage.TryGetValue(username, out var perStage)
                ? perStage.GetValueOrDefault(ss.Stagenr, 0)
                : 0;

            DB.StageStats.Add(new StageStats
            {
                RaceId = raceId,
                Stagenr = ss.Stagenr,
                Username = username,
                BudgetParticipation = budgetParticipation,
                StageScore = ss.StageScore,
                CumulativeScore = ss.TotalScore,
                MissedPoints = missed,
                DnfCount = absentPicks.Count,
                DnfBudget = absentPicks.Sum(p => p.Price),
                UpdatedAt = now,
            });
            inserted++;
        }
        DB.SaveChanges();
        return inserted;
    }

    private void SaveRacePayload(int raceId, bool budgetParticipation, RacePayload payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOpts);
        try
        {
            DB.RaceStats.Add(new RaceStats
            {
                RaceId = raceId,
                BudgetParticipation = budgetParticipation,
                Payload = json,
                UpdatedAt = DateTimeOffset.UtcNow,
            });
            DB.SaveChanges();
        }
        catch (DbUpdateException)
        {
            DB.ChangeTracker.Clear();
        }
    }

    private RacePayload BuildRacePayload(int raceId, bool budgetParticipation)
    {
        var race = DB.Races.AsNoTracking().Single(r => r.RaceId == raceId);

        var participants = DB.AccountParticipations.AsNoTracking()
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .Select(ap => new
            {
                ap.AccountParticipationId,
                Username = ap.Account.Username,
                FinalScore = ap.FinalScore ?? 0,
            })
            .ToList();

        var stageSelections = DB.StageSelections.AsNoTracking()
            .Where(ss => ss.Stage.RaceId == raceId
                      && ss.Stage.Finished
                      && ss.AccountParticipation.BudgetParticipation == budgetParticipation)
            .Select(ss => new
            {
                ss.AccountParticipationId,
                Stagenr = ss.Stage.Stagenr,
                StageType = ss.Stage.Type,
                ss.StageId,
                ss.StageScore,
                ss.TotalScore,
                ss.KopmanId,
            })
            .ToList();

        var realStages = stageSelections.Where(s => s.StageType != StageType.FinalStandings).ToList();

        var stageWins = participants.ToDictionary(p => p.AccountParticipationId, _ => 0);
        var redLanterns = participants.ToDictionary(p => p.AccountParticipationId, _ => 0);
        foreach (var grp in realStages.GroupBy(s => s.Stagenr))
        {
            var maxScore = grp.Max(g => g.StageScore);
            var minScore = grp.Min(g => g.StageScore);
            foreach (var s in grp.Where(g => g.StageScore == maxScore)) stageWins[s.AccountParticipationId]++;
            foreach (var s in grp.Where(g => g.StageScore == minScore)) redLanterns[s.AccountParticipationId]++;
        }

        var cumulativePer = realStages
            .GroupBy(s => s.AccountParticipationId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(s => s.Stagenr, s => s.TotalScore));

        var maxStageScore = realStages.GroupBy(s => s.AccountParticipationId)
            .ToDictionary(g => g.Key, g => g.Max(s => s.StageScore));
        var minStageScore = realStages.GroupBy(s => s.AccountParticipationId)
            .ToDictionary(g => g.Key, g => g.Min(s => s.StageScore));

        var kopmanRows = (from ss in DB.StageSelections.AsNoTracking()
                          where ss.Stage.RaceId == raceId
                             && ss.Stage.Finished
                             && ss.AccountParticipation.BudgetParticipation == budgetParticipation
                             && ss.KopmanId != null
                          from rp in DB.ResultsPoints
                          where rp.StageId == ss.StageId && rp.RiderParticipationId == ss.KopmanId
                          select new { ss.AccountParticipationId, Bonus = (rp.StageScore ?? 0) * 0.5 })
            .ToList();
        var kopmanBonusPer = kopmanRows
            .GroupBy(x => x.AccountParticipationId)
            .ToDictionary(g => g.Key, g => (int)g.Sum(x => x.Bonus));

        var missedPerUsername = Statistics.MissedPoints(raceId, budgetParticipation)
            .ToDictionary(m => m.Username, m => m);
        var uitvallersPerUsername = Statistics.Uitvallers(raceId, budgetParticipation)
            .ToDictionary(u => u.UserName, u => u);

        var rankedParticipants = participants.OrderByDescending(p => p.FinalScore).ToList();
        var finalRank = new Dictionary<int, int>();
        for (int i = 0; i < rankedParticipants.Count; i++)
        {
            finalRank[rankedParticipants[i].AccountParticipationId] = i + 1;
        }

        var players = participants.Select(p =>
        {
            var apId = p.AccountParticipationId;
            var bonus = kopmanBonusPer.GetValueOrDefault(apId, 0);
            var totalForPct = p.FinalScore > 0 ? p.FinalScore : 0;
            var missed = missedPerUsername.GetValueOrDefault(p.Username);
            var uitval = uitvallersPerUsername.GetValueOrDefault(p.Username);
            var missedTotalRow = missed?.Data.FirstOrDefault(d => d.Etappe == "Totaal");

            return new RacePayloadPlayer(
                Username: p.Username,
                FinalScore: p.FinalScore,
                FinalRank: finalRank[apId],
                KopmanBonus: bonus,
                KopmanPctOfTotal: totalForPct > 0 ? (double)bonus / totalForPct : 0,
                StageWins: stageWins.GetValueOrDefault(apId, 0),
                RedLanterns: redLanterns.GetValueOrDefault(apId, 0),
                TotalGemist: missedTotalRow?.Gemist ?? 0,
                TotalOptimal: missedTotalRow?.Optimaal ?? 0,
                MissedPointsPerStage: missed?.Data ?? new List<MissedPointsData>(),
                DnfRiderCount: uitval?.Uitvallers ?? 0,
                DnfBudgetLost: uitval?.UitvallerBudget ?? 0,
                UitvallerStagesBudget: uitval?.UitvallerStagesBudget ?? 0,
                MaxStageScore: maxStageScore.GetValueOrDefault(apId, 0),
                MinStageScore: minStageScore.GetValueOrDefault(apId, 0),
                CumulativeAtStage: cumulativePer.GetValueOrDefault(apId, new Dictionary<int, int>())
            );
        }).ToList();

        return new RacePayload(race.RaceId, race.Name, race.Year, players);
    }
}
