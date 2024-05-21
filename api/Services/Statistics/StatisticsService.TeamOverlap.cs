using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> Uniekheid(int raceId, bool budgetParticipation, bool includeDnf)
    {
        var participants = DB.AccountParticipations.Count(x => x.RaceId == raceId && x.BudgetParticipation == budgetParticipation) - 1;

        var counts = from rp in DB.RiderParticipations
                     join tsr in DB.TeamSelections on rp.RiderParticipationId equals tsr.RiderParticipationId
                     join ap in DB.AccountParticipations on tsr.AccountParticipationId equals ap.AccountParticipationId
                     where DB.TeamSelections.Select(x => x.RiderParticipationId).Contains(rp.RiderParticipationId)
                         && ap.BudgetParticipation == false && (rp.Dnf == false || rp.Dnf == includeDnf)
                     group new { rp, tsr } by rp.RiderParticipationId into grp
                     select new
                     {
                         rpId = grp.Key,
                         Selected = grp.Count() - 1,
                         grp.First().rp.Price
                     };
        var budget = DB.RaceBudget(raceId, budgetParticipation) / 100;

        var query = from ap in DB.Accpars.Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
                    join ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account) on ap.AccountParticipationId equals ts.AccountParticipationId
                    join rider in counts on ts.RiderParticipationId equals rider.rpId
                    group new { ap, ts, rider } by ap into g
                    select new
                    {
                        User = g.Key.Username,
                        Uniekheid = g.Sum(x => x.rider.Price * (participants - x.rider.Selected)) / budget / participants
                    };
        return query.OrderByDescending(x => x.Uniekheid);
    }
}
