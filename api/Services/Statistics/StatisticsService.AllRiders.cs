using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> AllRiders(int raceId, bool budgetParticipation)
    {
        var query = DB.RiderParticipations
            .Include(rp => rp.Rider)
            .Include(rp => rp.ResultsPoints)
            .Include(rp => rp.AccountParticipations).ThenInclude(ap => ap.Account)
            .AsNoTracking()
            .Where(rp => rp.RaceId == raceId && rp.Price <= (budgetParticipation ? 750000 : int.MaxValue))
            .Select(rp => new
            {
                RiderParticipation = rp,
                StageScore = rp.ResultsPoints.Sum(x => x.StageScore),
                Klassementen = rp.ResultsPoints.Sum(x => x.Gc.Score + x.Points.Score + x.Kom.Score + x.Youth.Score),
                TeamScore = rp.ResultsPoints.Sum(x => x.Teamscore),
                TotalScore = budgetParticipation ? rp.ResultsPoints.Sum(x => x.Totalscore - x.Teamscore) : rp.ResultsPoints.Sum(x => x.Totalscore),
                TotalSelected = rp.AccountParticipations.Count(ap => ap.BudgetParticipation == budgetParticipation),
                Accounts = rp.AccountParticipations.Where(ap => ap.BudgetParticipation == budgetParticipation).Select(x => x.Account.Username)
            });

        return query.OrderByDescending(x => x.TotalScore).ToList();
    }
}
