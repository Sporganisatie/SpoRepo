using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;

namespace SpoRE.Infrastructure.Database;

public class TeamSelectionClient
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;
    public TeamSelectionClient(DatabaseContext databaseContext, Userdata userData)
    {
        DB = databaseContext;
        User = userData;
    }

    internal IEnumerable<RiderParticipation> GetTeam(int raceId, bool budgetParticipation) // Alleen rp.id returnen kan wss ook
    {
        var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                    let ap = ts.AccountParticipation
                    where ap.RaceId == raceId && ap.AccountId == User.Id && ap.Budgetparticipation == budgetParticipation
                    select ts.RiderParticipation;
        return query.ToList(); // TODO handle errors and return Result<T>
    }

    internal List<RiderParticipation> GetAll(int raceId, int maxPrice)
        => DB.RiderParticipations
            .Include(rp => rp.Rider)
            .Where(rp => rp.RaceId == raceId && rp.Price < maxPrice)
            .ToList(); // TODO handle errors and return Result<T>

    internal Race GetRaceInfo(int raceId) // TODO misschien in race client, handle errors
        => DB.Races.Single(r => r.RaceId == raceId);

    internal RiderParticipation GetRider(int riderParticipationId)
        => DB.RiderParticipations
            .Single(rp => rp.RiderParticipationId == riderParticipationId);

    internal void AddRider(int riderParticipationId, int raceId, bool budgetParticipation)
    {
        DB.TeamSelections.Add(
            new()
            {
                RiderParticipationId = riderParticipationId,
                AccountParticipationId =
                    DB.AccountParticipations
                        .Single(ap => ap.RaceId == raceId && ap.AccountId == User.Id && ap.Budgetparticipation == budgetParticipation).AccountParticipationId
            });
        DB.SaveChanges();
    }
}