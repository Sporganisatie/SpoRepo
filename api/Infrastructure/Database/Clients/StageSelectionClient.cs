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

    // TODO move logic, nu beetje aparte combinatie van queries en processing die eigelijk in service hoort

    internal StageSelectionData GetData(int raceId, int stagenr)
    {
        var team = GetTeam(stagenr);
        var stageInfo = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        var topClassifications = GetTop5s(raceId, stagenr);
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

    private Classifications GetTop5s(int raceId, int stagenr)
    {
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();

        var mostRecentStage = DB.Stages.OrderByDescending(s => s.Stagenr).First(s => s.Finished && s.RaceId == raceId);

        var gcStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Gcpos > 0).OrderBy(rp => rp.Gcpos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Position = rp.Gcpos,
                              Result = rp.Gcresult,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var pointsStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Pointspos > 0).OrderBy(rp => rp.Pointspos)
                              select new ClassificationRow
                              {
                                  Rider = rp.RiderParticipation.Rider,
                                  Position = rp.Pointspos,
                                  Result = rp.Pointsresult,
                                  Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                              };

        var komStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Kompos > 0).OrderBy(rp => rp.Kompos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Position = rp.Kompos,
                               Result = rp.Komresult,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        var yocStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Yocpos > 0).OrderBy(rp => rp.Yocpos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Position = rp.Yocpos,
                               Result = rp.Yocresult,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };
        //   Selected = stageSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
        return new(gcStandings.Take(5).ToList(), pointsStandings.Take(5).ToList(), komStandings.Take(5).ToList(), yocStandings.Take(5).ToList());
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
