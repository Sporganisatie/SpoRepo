using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using Z.EntityFramework.Plus;

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

    internal Race GetRaceInfo(int raceId) // TODO misschien in race client, handle errors
        => DB.Races.Single(r => r.RaceId == raceId);

    internal RiderParticipation GetRider(int riderParticipationId, int raceId)
        => DB.RiderParticipations
            .Single(rp => rp.RiderParticipationId == riderParticipationId && rp.RaceId == raceId);  // TODO handle errors and return Result<T>

    internal int AddRider(int riderParticipationId)
    {
        DB.TeamSelections.Add(
            new()
            {
                RiderParticipationId = riderParticipationId,
                AccountParticipationId = User.ParticipationId
            });
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int RemoveRider(int riderParticipationId)
    {
        var selectionRiders = DB.StageSelectionRiders
            .Where(sr => sr.StageSelection.AccountParticipationId == User.ParticipationId && sr.RiderParticipationId == riderParticipationId);
        DB.StageSelectionRiders.RemoveRange(selectionRiders);

        DB.TeamSelections.Remove(new() { RiderParticipationId = riderParticipationId, AccountParticipationId = User.ParticipationId });

        DB.StageSelections
            .Where(s => s.AccountParticipationId == User.ParticipationId && s.KopmanId == riderParticipationId)
            .Update(s => new StageSelection { KopmanId = null });

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    // TODO move to different client
    public Task<Result> JoinRace(int raceId)
    {
        string budgetInsert = User.Id <= 5 ? $",({User.Id},{raceId},true)" : "";

        string accountParticipationQuery = $@"INSERT INTO account_participation(account_id, race_id, budgetparticipation)
                VALUES({User.Id}, {raceId}, false) {budgetInsert}
                ON CONFLICT (account_id, race_id, budgetparticipation) DO UPDATE SET budgetparticipation = EXCLUDED.budgetparticipation";

        var rowsAffected = DB.Database.ExecuteSqlRaw(accountParticipationQuery);
        string stage_selectionQuery = "INSERT INTO stage_selection(stage_id, account_participation_id) VALUES";
        for (int stage = 1; stage < 23; stage++)
        {
            string stage_id = $"(SELECT stage_id FROM stage WHERE race_id = {raceId} AND stagenr = {stage})";
            string budgparticipationInsert = "";
            if (User.Id <= 5)
            {
                budgparticipationInsert = $",({stage_id},(SELECT account_participation_id FROM account_participation WHERE race_id = {raceId} AND account_id = {User.Id} AND budgetparticipation = true))";
            }
            stage_selectionQuery += $"({stage_id},(SELECT account_participation_id FROM account_participation WHERE race_id = {raceId} AND account_id = {User.Id} AND budgetparticipation = false)){budgparticipationInsert},";
        }

        stage_selectionQuery = stage_selectionQuery.Remove(stage_selectionQuery.Length - 1) + " ON CONFLICT (account_participation_id, stage_id) DO NOTHING;";
        var rowsAffected2 = DB.Database.ExecuteSqlRaw(stage_selectionQuery);
        return Task.FromResult(Result.OK);
    }
}
