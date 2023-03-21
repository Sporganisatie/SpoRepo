using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Database;

public class TeamSelectionClient
{
    DatabaseContext DB;
    public TeamSelectionClient(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public IEnumerable<RiderParticipation> GetTeam(int accountId, int raceId, bool budgetParticipation)
    {
        // TODO if race started return (specific redirect) error
        var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider) // TODO fix infinite nesting
                    let ap = ts.AccountParticipation
                    where ap.RaceId == raceId && ap.AccountId == accountId && ap.Budgetparticipation == budgetParticipation
                    select ts.RiderParticipation;
        return query.ToList();
    }

    private static bool IsAccountParticipationMatch(AccountParticipation ap, int raceId, int accountId, bool budgetParticipation)
        => ap.RaceId == raceId && ap.AccountId == accountId && ap.Budgetparticipation == budgetParticipation;

    public List<RiderParticipation> GetAll(int accountId, int raceId, bool budgetParticipation)
    {
        // TODO if race started return (specific redirect) error
        var maxPrice = budgetParticipation ? 750_000 : int.MaxValue;
        var query = from rp in DB.RiderParticipations.Include(rp => rp.Rider) // TODO fix infinite nesting
                    where rp.RaceId == raceId && rp.Price < maxPrice
                    select rp; // TODO Selectable Enum based on price and already selected
        var output = query.ToList();
        return output;
    }
}