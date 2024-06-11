using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using Z.EntityFramework.Plus;

namespace SpoRE.Services.StageSelection;

public record StageSelectionData(IEnumerable<StageSelectableRider> Team, DateTime? Deadline, Classifications Classifications, int Compleet, int? BudgetCompleet);

public class StageSelectionService(DatabaseContext DB, Userdata User, StageResultService StageResultService)
{
    internal StageSelectionData GetData(int raceId, int stagenr)
    {
        var team = GetTeam(stagenr).OrderBy(x => x.Rider.Dnf).ThenBy(x => !x.Selected).ThenBy(x => x.Rider.Type).ThenByDescending(x => x.Rider.Price).ThenBy(x => x.Rider.Rider.Lastname);
        var stageInfo = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        var mostRecentFinished = DB.Stages.OrderByDescending(s => s.Stagenr).FirstOrDefault(s => s.Finished && s.RaceId == raceId);
        var topClassifications = mostRecentFinished is null ? Classifications.Empty : StageResultService.GetClassifications(mostRecentFinished, top5: true, stagenr);
        return new StageSelectionData(team, stageInfo.Starttime, topClassifications, OpstellingCompleet(raceId, stagenr, false).Value, OpstellingCompleet(raceId, stagenr, true));
    }

    private int? OpstellingCompleet(int raceId, int stagenr, bool budget)
    {
        if (budget && User.Id > 5) return null;
        var stageSelection = DB.StageSelections.Where(ss => ss.AccountParticipation.AccountId == User.Id && ss.AccountParticipation.BudgetParticipation == budget && ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr)
            .Join(DB.StageSelectionRiders, ss => ss.StageSelectionId, ssr => ssr.StageSelectionId, (ss, ssr) => new { Kopman = ss.KopmanId == ssr.RiderParticipationId }).ToList();
        return stageSelection.Count + (stageSelection.Any(r => r.Kopman) ? 1 : 0);
    }

    private List<StageSelectableRider> GetTeam(int stagenr)
    {
        var stageSelection = DB.StageSelections.Where(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr)
            .Join(DB.StageSelectionRiders, ss => ss.StageSelectionId, ssr => ssr.StageSelectionId, (ss, ssr) => new { ssr.RiderParticipationId, Kopman = ss.KopmanId == ssr.RiderParticipationId });

        var team = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                   let ap = ts.AccountParticipation
                   let selected = stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId)
                   let isKopman = stageSelection.Any(ss => ss.RiderParticipationId == ts.RiderParticipationId && ss.Kopman)
                   where ap.AccountParticipationId == User.ParticipationId
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
