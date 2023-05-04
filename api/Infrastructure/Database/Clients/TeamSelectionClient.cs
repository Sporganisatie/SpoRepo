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

    internal IEnumerable<RiderParticipation> GetTeam() // Alleen rp.id returnen kan wss ook
    {
        var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                    let ap = ts.AccountParticipation
                    where ap.AccountParticipationId == User.ParticipationId
                    select ts.RiderParticipation;
        return query.ToList(); // TODO handle errors and return Result<T>
    }

    internal List<RiderParticipation> GetAll(int raceId, int maxPrice)
        => DB.RiderParticipations
            .Include(rp => rp.Rider)
            .Where(rp => rp.RaceId == raceId && rp.Price < maxPrice)
            .ToList(); // TODO handle errors and return Result<T>

    internal Task<Result<Race>> GetRaceInfo(int raceId) // TODO misschien in race client, handle errors
        => DB.Races.SingleResult(r => r.RaceId == raceId);

    internal RiderParticipation GetRider(int riderParticipationId, int raceId)
        => DB.RiderParticipations
            .Single(rp => rp.RiderParticipationId == riderParticipationId && rp.RaceId == raceId);  // TODO handle errors and return Result<T>

    internal Task<int> AddRider(int riderParticipationId)
    {
        DB.TeamSelections.Add(
            new()
            {
                RiderParticipationId = riderParticipationId,
                AccountParticipationId = User.ParticipationId
            });
        return DB.SaveChangesAsync();
        // if (a != 1) return Result.WithMessages(ValidationMessage.Error("did not add 1 rider"));
        // return Result.OK;
    }

    internal int RemoveRider(int riderParticipationId)
    {
        DB.TeamSelections.Remove(new() { RiderParticipationId = riderParticipationId, AccountParticipationId = User.ParticipationId });
        // TODO remove rider from stage selections, dit mag met automatische chaining als dat makkelijk kan
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }
}