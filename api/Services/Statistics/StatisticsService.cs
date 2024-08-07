using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public partial class StatisticsService(DatabaseContext DB)
{
    public record UitvallersData(string UserName, int Uitvallers, int UitvallerBudget);

    public IEnumerable<UitvallersData> Uitvallers(int raceId, bool budgetParticipation)
    {
        var query = from ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account).Include(ts => ts.RiderParticipation)
                    where ts.RiderParticipation.Dnf && ts.AccountParticipation.RaceId == raceId && ts.AccountParticipation.BudgetParticipation == budgetParticipation
                    group ts by ts.AccountParticipation.Account.Username into g
                    select new UitvallersData(g.Key, g.Count(), g.Sum(ts => ts.RiderParticipation.Price));

        return query.ToList().OrderByDescending(x => x.Uitvallers).ThenByDescending(x => x.UitvallerBudget);
    }

    public object Klassementen(int raceId, bool budgetParticipation)
    {
        var mostRecentFinished = DB.Stages.OrderByDescending(s => s.Stagenr).FirstOrDefault(s => s.Finished && s.RaceId == raceId);
        var gcQuery = (from points in DB.ResultsPoints.Where(points => points.StageId == mostRecentFinished.StageId)
                       join ts in DB.TeamSelections.Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on points.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                       from ts in tsGroup.DefaultIfEmpty()
                       where points.Gc.Position > 0
                       group new { points, ts } by points into g
                       orderby g.First().points.Gc.Position
                       select new
                       {
                           g.Key.Gc.Position,
                           g.Key.Gc.Result,
                           g.First().points.RiderParticipation.Rider,
                           g.First().points.RiderParticipation.Price,
                           Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                       }).Take(20).ToList();

        var PointsQuery = (from points in DB.ResultsPoints.Where(points => points.StageId == mostRecentFinished.StageId)
                           join ts in DB.TeamSelections.Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on points.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                           from ts in tsGroup.DefaultIfEmpty()
                           where points.Points.Position > 0
                           group new { points, ts } by points into g
                           orderby g.First().points.Points.Position
                           select new
                           {
                               g.Key.Points.Position,
                               g.Key.Points.Result,
                               g.First().points.RiderParticipation.Rider,
                               g.First().points.RiderParticipation.Price,
                               Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                           }).Take(20).ToList();

        var KomQuery = (from points in DB.ResultsPoints.Where(points => points.StageId == mostRecentFinished.StageId)
                        join ts in DB.TeamSelections.Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on points.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                        from ts in tsGroup.DefaultIfEmpty()
                        where points.Kom.Position > 0
                        group new { points, ts } by points into g
                        orderby g.First().points.Kom.Position
                        select new
                        {
                            g.Key.Kom.Position,
                            g.Key.Kom.Result,
                            g.First().points.RiderParticipation.Rider,
                            g.First().points.RiderParticipation.Price,
                            Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                        }).Take(20).ToList();

        var YouthQuery = (from points in DB.ResultsPoints.Where(points => points.StageId == mostRecentFinished.StageId)
                          join ts in DB.TeamSelections.Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on points.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                          from ts in tsGroup.DefaultIfEmpty()
                          where points.Youth.Position > 0
                          group new { points, ts } by points into g
                          orderby g.First().points.Youth.Position
                          select new
                          {
                              g.Key.Youth.Position,
                              g.Key.Youth.Result,
                              g.First().points.RiderParticipation.Rider,
                              g.First().points.RiderParticipation.Price,
                              Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                          }).Take(20).ToList();
        return new object[4] { gcQuery, PointsQuery, KomQuery, YouthQuery };
    }
}
