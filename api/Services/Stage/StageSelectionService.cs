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
        var stageSelection = DB.StageSelections.Include(ss => ss.RiderParticipations)
        .Single(ss => ss.AccountParticipation.AccountId == User.Id && ss.AccountParticipation.BudgetParticipation == budget && ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr);
        return stageSelection.RiderParticipations.Count + (stageSelection.KopmanId is not null ? 1 : 0);
    }

    private List<StageSelectableRider> GetTeam(int stagenr)
    {
        var stageSelection = DB.StageSelections.Include(ss => ss.RiderParticipations).Single(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr);

        return DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking()
            .Single(ap => ap.AccountParticipationId == User.ParticipationId).RiderParticipations
            .Select(rp => new StageSelectableRider(
                    rp,
                    stageSelection.RiderParticipations.Contains(rp),
                    stageSelection.KopmanId == rp.RiderParticipationId)).ToList();
    }

    internal int AddRider(int riderParticipationId, int stagenr)
    {
        var stageSelection = DB.StageSelections.Include(ss => ss.RiderParticipations).Single(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr);

        if (stageSelection.RiderParticipations.Count >= 9) return 0;

        var riderToAdd = DB.RiderParticipations.Single(rp => rp.RiderParticipationId == riderParticipationId);
        stageSelection.RiderParticipations.Add(riderToAdd);

        return DB.SaveChanges();  // TODO handle errors and return Result<T>
    }

    internal int SetKopman(int riderParticipationId, int stagenr)
    {
        var stageSelection = DB.StageSelections.Include(ss => ss.RiderParticipations).Single(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr);
        if (!stageSelection.RiderParticipations.Any(x => x.RiderParticipationId == riderParticipationId)) return 0;
        stageSelection.KopmanId = riderParticipationId;
        return DB.SaveChanges();
    }

    internal int RemoveRider(int riderParticipationId, int stagenr)
    {
        var stageSelection = DB.StageSelections.Include(ss => ss.RiderParticipations)
            .Single(s => s.AccountParticipationId == User.ParticipationId && s.Stage.Stagenr == stagenr);

        var riderParticipation = stageSelection.RiderParticipations
            .SingleOrDefault(rp => rp.RiderParticipationId == riderParticipationId);

        stageSelection.RiderParticipations.Remove(riderParticipation);

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
