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
        DB.TeamSelections.Remove(new() { RiderParticipationId = riderParticipationId, AccountParticipationId = User.ParticipationId });
        // TODO remove rider from stage selections, dit mag met automatische chaining als dat makkelijk kan
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }


    // public Task<Result<AccountParticipation>> JoinRace(int raceId)
    // {
    //     string budgetInsert = User.Id <= 5 ? $",({User.Id},{raceId},true)" : "";

    //     string account_participationQuery = $@"INSERT INTO account_participation(account_id, race_id, budgetparticipation)
    //             VALUES({User.Id}, {raceId}, false) {budgetInsert}
    //             ON CONFLICT (account_id, race_id, budgetparticipation) DO NOTHING
    //             RETURNING (account_participation_id);
    //             SELECT COUNT(*) FROM stage WHERE race_id = {raceId};";

    //     var results = DB.;
    //     if ((results[0].Rows.Count == 2 && User.Id <= 5) || (results[0].Rows.Count == 1 && User.Id > 5))
    //     {
    //         string stage_selectionQuery = "INSERT INTO stage_selection(stage_id, account_participation_id) VALUES";
    //         for (int stage = 1; stage < results[1].Rows[0]["count"] + 1; stage++)
    //         {
    //             string stage_id = $"(SELECT stage_id FROM stage WHERE race_id = {raceId} AND stagenr = {stage})";
    //             string budgparticipationInsert = "";
    //             if (User.Id <= 5)
    //             {
    //                 budgparticipationInsert = $",({stage_id},{results[0].Rows[1]["account_participation_id"]})";
    //             }
    //             stage_selectionQuery += $"({stage_id},{results[0].Rows[0]["account_participation_id"]}){budgparticipationInsert},";
    //         }

    //         stage_selectionQuery = stage_selectionQuery.Remove(stage_selectionQuery.Length - 1) + "ON CONFLICT (account_participation_id, stage_id) DO NOTHING;";
    //     }
    // }

}