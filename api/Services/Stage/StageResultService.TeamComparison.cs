using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using static SpoRE.Helper.HelperFunctions;

namespace SpoRE.Services;

public partial class StageResultService
{
    public TeamSelections AllStageSelections(int raceId, bool budgetParticipation, int stagenr)
    {
        if (DB.Stages.Single(s => s.RaceId == raceId && s.Stagenr == stagenr).Starttime > DateTime.UtcNow) return new([], []);

        var teamSelection = DB.AccountParticipations.Include(ap => ap.RiderParticipations).AsNoTracking().Single(ts => ts.AccountParticipationId == User.ParticipationId).RiderParticipations
            .Select(rp => rp.RiderParticipationId).ToList();
        var userStageSelection = DB.StageSelections.Include(ap => ap.RiderParticipations).AsNoTracking().Single(ss => ss.AccountParticipationId == User.ParticipationId && ss.Stage.Stagenr == stagenr).RiderParticipations.Select(ssr => ssr.RiderParticipationId).ToList();
        var allStageSelections = DB.StageSelections
            .Include(ss => ss.AccountParticipation.Account)
            .Include(ss => ss.RiderParticipations).ThenInclude(rp => rp.Rider)
            .AsNoTracking()
            .Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation).ToList().OrderByDescending(x => x.TotalScore);

        var results = DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr && rp.Stage.RaceId == raceId).AsNoTrackingWithIdentityResolution().ToList();

        var allSelected = DB.StageSelections.Include(ss => ss.RiderParticipations).AsNoTracking().Where(ss => ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr).SelectMany(ss => ss.RiderParticipations).Select(rp => rp.RiderParticipationId).ToList();
        var output = new List<UserSelection>();
        foreach (var stageSelection in allStageSelections)
        {
            var selectedRiders = stageSelection.RiderParticipations
                .GroupJoin(
                    results,
                    rp => rp.RiderParticipationId,
                    res => res.RiderParticipationId,
                    (rp, resGroup) => new { RiderParticipation = rp, Result = resGroup.DefaultIfEmpty() } // Left join
                )
                .SelectMany(
                    joined => joined.Result.Select(res => new StageComparisonRider
                    {
                        Rider = joined.RiderParticipation.Rider,
                        Kopman = joined.RiderParticipation.RiderParticipationId == stageSelection.KopmanId,
                        StagePos = res?.StagePos, // Default value if result is null
                        TotalScore = (budgetParticipation ? ((res?.Totalscore ?? 0) - (res?.Teamscore ?? 0)) : (res?.Totalscore ?? 0))
                            + (int)(joined.RiderParticipation.RiderParticipationId == stageSelection.KopmanId ? res?.StageScore * 0.5 : 0),
                        Selected = userStageSelection.Contains(joined.RiderParticipation.RiderParticipationId)
                            ? StageSelectedEnum.InStageSelection
                            : teamSelection.Contains(joined.RiderParticipation.RiderParticipationId)
                                ? StageSelectedEnum.InTeam
                                : StageSelectedEnum.None,
                        // Dnf = joined.RiderParticipation.Dnf // TODO: Only use if StagePos is empty, mainly for UI changes
                    })
                );

            var riderScores = selectedRiders.OrderBy(r => r.StagePos).ToList();

            var heleTeam = DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking()
                .Single(ap => ap.AccountParticipationId == stageSelection.AccountParticipationId).RiderParticipations
                .GroupJoin(
                    results,
                    rp => rp.RiderParticipationId,
                    res => res.RiderParticipationId,
                    (rp, resGroup) => new { RiderParticipation = rp, Results = resGroup.DefaultIfEmpty() } // Left join
                )
                .SelectMany(
                    joined => joined.Results.Select(res => new
                    {
                        joined.RiderParticipation,
                        Result = res ?? new ResultsPoint { Totalscore = 0, Teamscore = 0, StagePos = null }
                    })
                );

            var gemist = heleTeam.Where(rp => !stageSelection.RiderParticipations.Contains(rp.RiderParticipation) &&
                (allSelected.Contains(rp.RiderParticipation.RiderParticipationId) || ((budgetParticipation ? (rp.Result.Totalscore - rp.Result.Teamscore) : rp.Result.Totalscore) ?? 0) > 0))
                    .Select(rp => new StageComparisonRider
                    {
                        Rider = rp.RiderParticipation.Rider,
                        StagePos = rp.Result.StagePos,
                        TotalScore = (budgetParticipation ? (rp.Result.Totalscore - rp.Result.Teamscore) : rp.Result.Totalscore) ?? 0,
                        Selected = userStageSelection.Contains(rp.RiderParticipation.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipation.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None,
                        // Dnf = rp.RiderParticipation.Dnf // TODO alleen gebruiken als stagePos empty, vooral ui change
                    }).ToList();

            output.Add(new UserSelection(stageSelection.AccountParticipation.Account.Username, riderScores, gemist));
        }

        return OrderSelectedRiders(output);
    }
}