using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public partial class StatisticsService
{
    private readonly DatabaseContext DB;

    public StatisticsService(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public record UitvallersData(string UserName, int Uitvallers, int UitvallerBudget);

    public IEnumerable<UitvallersData> Uitvallers(int raceId, bool budgetParticipation)
    {
        var query = from ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account).Include(ts => ts.RiderParticipation)
                    where ts.RiderParticipation.Dnf && ts.AccountParticipation.RaceId == raceId && ts.AccountParticipation.BudgetParticipation == budgetParticipation
                    group ts by ts.AccountParticipation.Account.Username into g
                    select new UitvallersData(g.Key, g.Count(), g.Sum(ts => ts.RiderParticipation.Price));

        return query.ToList().OrderByDescending(x => x.Uitvallers).ThenByDescending(x => x.UitvallerBudget);
    }
}
