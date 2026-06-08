using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class AccountParticipationStatsService(DatabaseContext DB)
{
    public async Task Calculate(int raceId)
    {
        var race = await DB.Races.AsNoTracking().SingleAsync(r => r.RaceId == raceId);
        if (!race.Finished) return;

        var accountParticipations = await DB.AccountParticipations
            .Include(ap => ap.AccountParticipationStats)
            .Where(ap => ap.RaceId == raceId)
            .ToListAsync();

        var gemistByAp = await DB.StageSelections
            .Where(ss => ss.Stage.RaceId == raceId && ss.StageSelectionStats != null)
            .GroupBy(ss => ss.AccountParticipationId)
            .Select(g => new { AccountParticipationId = g.Key, TotaalGemist = g.Sum(ss => ss.StageSelectionStats.Gemist) })
            .ToDictionaryAsync(x => x.AccountParticipationId, x => x.TotaalGemist);

        ApplyRanking(accountParticipations.Where(ap => ap.BudgetParticipation).ToList(), gemistByAp);
        ApplyRanking(accountParticipations.Where(ap => !ap.BudgetParticipation).ToList(), gemistByAp);

        await DB.SaveChangesAsync();
    }

    private static void ApplyRanking(List<AccountParticipation> participations, Dictionary<int, int> gemistByAp)
    {
        var ordered = participations.OrderByDescending(ap => ap.FinalScore ?? 0).ToList();
        var rank = 0;
        var extraRankIncreaseAfterTie = 0;
        for (int i = 0; i < ordered.Count; i++)
        {
            var accPar = ordered[i];
            if (rank == 0 || (accPar.FinalScore ?? 0) < (ordered[i - 1].FinalScore ?? 0))
            {
                rank += 1 + extraRankIncreaseAfterTie;
                extraRankIncreaseAfterTie = 0;
            }
            else
            {
                extraRankIncreaseAfterTie++;
            }

            var stats = accPar.AccountParticipationStats ?? new AccountParticipationStats();
            stats.Positie = rank;
            stats.TotaalGemist = gemistByAp.TryGetValue(accPar.AccountParticipationId, out var g) ? g : 0;
            accPar.AccountParticipationStats = stats;
        }
    }
}
