using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using Z.EntityFramework.Plus;

namespace SpoRE.Services.StageSelection;

public class StageSelectionService
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;
    private readonly StageResultService StageResultService;

    public StageSelectionService(DatabaseContext databaseContext, Userdata userData, StageResultService stageResultService)
    {
        DB = databaseContext;
        User = userData;
        StageResultService = stageResultService;
    }

    internal StageSelectionData GetData(int raceId, int stagenr)
    {
        var team = GetTeam(stagenr);
        var stageInfo = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        var mostRecentFinished = DB.Stages.OrderByDescending(s => s.Stagenr).FirstOrDefault(s => s.Finished && s.RaceId == raceId);
        var topClassifications = mostRecentFinished is null ? Classifications.Empty : StageResultService.GetClassifications(mostRecentFinished, top5: true);
        return new StageSelectionData(team, stageInfo.Starttime, topClassifications);
    }

    private IEnumerable<StageSelectableRider> GetTeam(int stagenr)
    {
        var stageSelection = DB.StageSelections.Where(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr)
            .Join(DB.StageSelectionRiders, ss => ss.StageSelectionId, ssr => ssr.StageSelectionId, (ss, ssr) => new { ssr.RiderParticipationId, Kopman = ss.KopmanId == ssr.RiderParticipationId });

        var team = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                   let ap = ts.AccountParticipation
                   let selected = stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId)
                   let isKopman = stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId && ss.Kopman)
                   where ap.AccountParticipationId == User.ParticipationId
                   orderby ts.RiderParticipation.Dnf, !isKopman, !selected, ts.RiderParticipation.Price descending
                   select new StageSelectableRider(
                    ts.RiderParticipation,
                    selected,
                    isKopman);

        return team.ToList();
    }

    internal int AddRider(int riderParticipationId, int stagenr)
    {
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

    internal int SetKopman(int riderParticipationId, int stagenr)
    {
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
        var riderToDelete = DB.StageSelectionRiders.Single(sr =>
            sr.StageSelection.AccountParticipationId == User.ParticipationId
            && sr.RiderParticipationId == riderParticipationId
            && sr.StageSelection.Stage.Stagenr == stagenr);

        DB.StageSelectionRiders.Remove(riderToDelete);

        DB.StageSelections
            .Where(s => s.AccountParticipationId == User.ParticipationId && s.Stage.Stagenr == stagenr && s.KopmanId == riderParticipationId)
            .Update(s => new StageSelectie { KopmanId = null });

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int RemoveKopman(int riderParticipationId, int stagenr)
    {
        DB.StageSelections
            .First(s => s.AccountParticipationId == User.ParticipationId && s.Stage.Stagenr == stagenr).KopmanId = null;

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }
}