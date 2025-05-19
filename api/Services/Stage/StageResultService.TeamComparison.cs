using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
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
            var selectedRiders = stageSelection.RiderParticipations.Join(
                results,
                rp => rp.RiderParticipationId,
                res => res.RiderParticipationId,
                (rp, res) => new { RiderParticipation = rp, Result = res }
            ).Select(rp => new StageComparisonRider
            {
                Rider = rp.RiderParticipation.Rider,
                Kopman = rp.RiderParticipation.RiderParticipationId == stageSelection.KopmanId,
                StagePos = rp.Result.StagePos,
                TotalScore = (budgetParticipation ? (rp.Result.Totalscore - rp.Result.Teamscore) : rp.Result.Totalscore) ?? 0,
                Selected = userStageSelection.Contains(rp.RiderParticipation.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipation.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None,
                // Dnf = rp.RiderParticipation.Dnf // TODO alleen gebruiken als stagePos empty, vooral ui change
            });

            var riderScores = selectedRiders.OrderBy(r => r.StagePos).ToList();

            var heleTeam = DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking()
                .Single(ap => ap.AccountParticipationId == stageSelection.AccountParticipationId).RiderParticipations.Join(
                    results,
                    rp => rp.RiderParticipationId,
                    res => res.RiderParticipationId,
                    (rp, res) => new { RiderParticipation = rp, Result = res }
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