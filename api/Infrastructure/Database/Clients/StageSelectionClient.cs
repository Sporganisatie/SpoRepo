using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Models.Response;
using Z.EntityFramework.Plus;

namespace SpoRE.Infrastructure.Database;

public class StageSelectionClient
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;
    public StageSelectionClient(DatabaseContext databaseContext, Userdata userData)
    {
        DB = databaseContext;
        User = userData;
    }

    // get classifications wss in andere client

    internal IEnumerable<StageSelectableRider> GetTeam(int stagenr)
    {
        var stageSelection = DB.StageSelections.Where(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr)
            .Join(DB.StageSelectionRiders, ss => ss.StageSelectionId, ssr => ssr.StageSelectionId, (ss, ssr) => new { ssr.RiderParticipationId, Kopman = ss.KopmanId == ssr.RiderParticipationId });

        var team = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                   let ap = ts.AccountParticipation
                   where ap.AccountParticipationId == User.ParticipationId
                   orderby ts.RiderParticipation.Price descending
                   select new StageSelectableRider(
                    ts.RiderParticipation,
                    stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId),
                    stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId && ss.Kopman));

        return team.ToList();
    }

    internal int AddRider(int riderParticipationId, int stagenr)
    {
        // TODO check stage niet gestart in Service
        var stageSelectionId = DB.StageSelections
            .Where(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr)
            .Select(ss => ss.StageSelectionId)
            .FirstOrDefault();

        if (DB.StageSelectionRiders.Count(ssr => ssr.StageSelectionId == stageSelectionId) >= 9) return 0;
        DB.StageSelectionRiders.Add(
            new()
            {
                RiderParticipationId = riderParticipationId,
                StageSelectionId = stageSelectionId
            });
        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int AddKopman(int riderParticipationId, int stagenr)
    {
        // TODO check stage niet gestart in Service
        if (DB.StageSelectionRiders.Count(ssr =>
            ssr.StageSelection.AccountParticipationId == User.ParticipationId
            && ssr.StageSelection.Stage.Stagenr == stagenr
            && ssr.RiderParticipationId == riderParticipationId) != 1) return 0;
        var stageSelection = DB.StageSelections.Single(ss => ss.Stage.Stagenr == stagenr && ss.AccountParticipationId == User.ParticipationId);
        stageSelection.KopmanId = riderParticipationId;
        return DB.SaveChanges();
    }

    internal int RemoveRider(int riderParticipationId, int stagenr)
    {
        // TODO check stage niet gestart in Service
        var riderToDelete = DB.StageSelectionRiders.Single(sr =>
            sr.StageSelection.AccountParticipationId == User.ParticipationId
            && sr.RiderParticipationId == riderParticipationId
            && sr.StageSelection.Stage.Stagenr == stagenr);

        DB.StageSelectionRiders.Remove(riderToDelete);

        DB.StageSelections
            .Where(s => s.AccountParticipationId == User.ParticipationId && s.Stage.Stagenr == stagenr && s.KopmanId == riderParticipationId)
            .Update(s => new StageSelection { KopmanId = null });

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int RemoveKopman(int riderParticipationId, int stagenr)
    {
        // TODO check stage niet gestart in Service
        DB.StageSelections
            .First(s => s.AccountParticipationId == User.ParticipationId && s.Stage.Stagenr == stagenr).KopmanId = null;

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }
}