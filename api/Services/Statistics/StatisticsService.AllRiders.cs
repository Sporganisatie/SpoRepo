using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> AllRiders(int raceId, bool budgetParticipation)
    {
        var query = from rp in DB.RiderParticipations.Include(rp => rp.Rider).Where(rp => rp.RaceId == raceId && rp.Price <= (budgetParticipation ? 750000 : int.MaxValue))
                    join rider in DB.Riders on rp.RiderId equals rider.RiderId
                    join repo in DB.ResultsPoints on rp.RiderParticipationId equals repo.RiderParticipationId into results
                    join ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account).Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on rp.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                    from ts in tsGroup.DefaultIfEmpty()
                    from points in results.DefaultIfEmpty()
                    group new { rp, rider, points, ts } by rp into g
                    select new
                    {
                        RiderParticipation = g.Key,
                        Rider = g.Select(x => x.rider).First(),
                        StageScore = g.Sum(x => x.points.StageScore) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        Klassementen = g.Sum(x => x.points.Gc.Score + x.points.Points.Score + x.points.Kom.Score + x.points.Youth.Score ?? 0) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TeamScore = g.Sum(x => x.points.Teamscore) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TotalScore = (budgetParticipation ? g.Sum(x => x.points.Totalscore) - g.Sum(x => x.points.Teamscore) : g.Sum(x => x.points.Totalscore)) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TotalSelected = g.Select(x => x.ts.AccountParticipationId).Distinct().Count(),
                        Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                    };
        return query.OrderByDescending(x => x.TotalScore).ToList();
    }
}
