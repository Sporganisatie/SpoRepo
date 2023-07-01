using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> AllRiders(int raceId, bool budgetParticipation)
    {
        var query = from rp in DB.RiderParticipations.Include(rp => rp.Rider).Where(rp => rp.RaceId == 28 && rp.Price <= (budgetParticipation ? 750000 : int.MaxValue))
                    join rider in DB.Riders on rp.RiderId equals rider.RiderId
                    join repo in DB.ResultsPoints on rp.RiderParticipationId equals repo.RiderParticipationId into results
                    from points in results.DefaultIfEmpty()
                    group new { rp, rider, points } by rp into g
                    select new
                    {
                        RiderParticipation = g.Key,
                        Rider = g.Select(x => x.rider),
                        StageScore = g.Sum(x => x.points.StageScore),
                        Klassementen = g.Sum(x => x.points.Gc.Score + x.points.Points.Score + x.points.Kom.Score + x.points.Youth.Score ?? 0),
                        TeamScore = g.Sum(x => x.points.Teamscore),
                        TotalScore = budgetParticipation ? g.Sum(x => x.points.Totalscore) - g.Sum(x => x.points.Teamscore) : g.Sum(x => x.points.Totalscore),
                    };

        return query.OrderByDescending(x => x.TotalScore).ToList();
    }
}
