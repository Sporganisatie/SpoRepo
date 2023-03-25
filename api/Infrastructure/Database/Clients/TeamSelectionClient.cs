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

    internal RiderParticipation GetRider(int riderParticipationId, int raceId)
        => DB.RiderParticipations
            .Single(rp => rp.RiderParticipationId == riderParticipationId && rp.RaceId == raceId);  // TODO handle errors and return Result<T>

    internal int AddRider(int riderParticipationId, int raceId, bool budgetParticipation)
    {
        DB.TeamSelections.Add(
            new()
            {
                RiderParticipationId = riderParticipationId,
                AccountParticipationId =
                    DB.AccountParticipations
                        .Single(ap => ap.RaceId == raceId && ap.AccountId == User.Id && ap.Budgetparticipation == budgetParticipation).AccountParticipationId // TODO AccountParticipationId ook op user zetten?
            });
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int RemoveRider(int riderParticipationId, int raceId, bool budgetParticipation)
    {
        var toRemove = DB.TeamSelections.Single(
            ts => ts.RiderParticipationId == riderParticipationId &&
                ts.AccountParticipationId ==
                    DB.AccountParticipations
                        .Single(ap => ap.RaceId == raceId && ap.AccountId == User.Id && ap.Budgetparticipation == budgetParticipation).AccountParticipationId);
        DB.TeamSelections.Remove(toRemove);
        // TODO remove rider from stage selections, dit mag met automatische chaining als dat makkelijk kan
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }
}