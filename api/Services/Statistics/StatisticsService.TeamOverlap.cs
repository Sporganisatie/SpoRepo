using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public UniekheidResponse Uniekheid(int raceId, bool budgetParticipation)
    {
        var participants = DB.AccountParticipations.Count(x => x.RaceId == raceId && x.BudgetParticipation == budgetParticipation) - 1;

        var counts = from rp in DB.RiderParticipations
                     join tsr in DB.TeamSelections on rp.RiderParticipationId equals tsr.RiderParticipationId
                     join ap in DB.AccountParticipations on tsr.AccountParticipationId equals ap.AccountParticipationId
                     where DB.TeamSelections.Select(x => x.RiderParticipationId).Contains(rp.RiderParticipationId)
                         && ap.BudgetParticipation == budgetParticipation
                     group new { rp, tsr } by rp.RiderParticipationId into grp
                     select new
                     {
                         rpId = grp.Key,
                         Selected = grp.Count() - 1,
                         grp.First().rp.Price,
                         grp.First().rp.Dnf,
                     };

        var budget = DB.RaceBudget(raceId, budgetParticipation) / 100;

        var uniekheden = (from ap in DB.Accpars.Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
                          join ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account) on ap.AccountParticipationId equals ts.AccountParticipationId
                          join rider in counts on ts.RiderParticipationId equals rider.rpId
                          group new { ap, ts, rider } by ap into g
                          select new
                          {
                              User = g.Key.Username,
                              Start = g.Sum(x => x.rider.Price * (participants - x.rider.Selected)) / budget / participants,
                              Huidig = g.Where(x => !x.rider.Dnf).Sum(x => x.rider.Price * (participants - x.rider.Selected)) / budget / participants
                          }).ToList();

        var start = uniekheden.Select(x => new UniekheidRow(x.User, x.Start)).OrderByDescending(x => x.Uniekheid);
        var huidig = uniekheden.Select(x => new UniekheidRow(x.User, x.Huidig)).OrderByDescending(x => x.Uniekheid);

        return new(start, huidig);
    }

    public OverlapResponse Overlap(int raceId, bool budgetParticipation)
    {
        var teamSelections = (from ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account).Include(ts => ts.RiderParticipation)
                              where ts.AccountParticipation.RaceId == raceId && ts.AccountParticipation.BudgetParticipation == budgetParticipation
                              group new { ts, ts.AccountParticipation.Account.Username } by ts.AccountParticipation.Account into g
                              orderby g.Key.AccountId
                              select new TeamSelectionGrouped
                              {
                                  User = g.Key.Username,
                                  Renners = g.Select(x => x.ts.RiderParticipation)
                              }).ToList();
        var budget = DB.RaceBudget(raceId, budgetParticipation) / 100;

        var overlap = teamSelections.Select(x => GetOverlap(x, teamSelections, budget: false, budget));
        var overlapBudget = teamSelections.Select(x => GetOverlap(x, teamSelections, budget: true, budget));

        return new(overlap, overlapBudget);
    }

    private static OverlapRow GetOverlap(TeamSelectionGrouped main, List<TeamSelectionGrouped> teamSelections, bool budget, int budgetAmount)
        => new(main.User, teamSelections.ToDictionary(x => x.User, x => CalculateOverlap(x, main, budget, budgetAmount)));

    private static int CalculateOverlap(TeamSelectionGrouped other, TeamSelectionGrouped main, bool budget, int budgetAmount)
    {
        if (main.User == other.User) return -1;
        return budget
            ? main.Renners.Intersect(other.Renners).Sum(x => x.Price) / budgetAmount
            : main.Renners.Intersect(other.Renners).Count();
    }

    private class TeamSelectionGrouped
    {
        public string User { get; set; }
        public IEnumerable<RiderParticipation> Renners { get; set; }
    }
}
public record UniekheidRow(string User, int Uniekheid);

public record UniekheidResponse(IEnumerable<UniekheidRow> Start, IEnumerable<UniekheidRow> Huidig);

public record OverlapRow(string User, Dictionary<string, int> Overlaps);

public record OverlapResponse(IEnumerable<OverlapRow> Overlap, IEnumerable<OverlapRow> OverlapBudget);