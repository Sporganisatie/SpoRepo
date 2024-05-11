using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Models.Response;
using static SpoRE.Helper.HelperFunctions;

namespace SpoRE.Services;

public partial class StageResultService
{
    public TeamSelections AllStageSelections(int raceId, bool budgetParticipation, int stagenr)
    {
        if (DB.Stages.Single(s => s.RaceId == raceId && s.Stagenr == stagenr).Starttime > DateTime.UtcNow) return new(new List<UserSelection>(), new List<RiderCount>() { });

        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var users = DB.StageSelections.Include(ss => ss.AccountParticipation.Account).Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation).Select(ss => new { ss.StageSelectionId, ss.AccountParticipation.Account.Username, ss.AccountParticipationId }).ToList();
        var allSelected = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipation.BudgetParticipation == budgetParticipation && ssr.StageSelection.Stage.RaceId == raceId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                        join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                        from rp in results.DefaultIfEmpty()
                        where ssr.StageSelection.StageSelectionId == user.StageSelectionId
                        select new StageComparisonRider
                        {
                            Rider = ssr.RiderParticipation.Rider,
                            Kopman = ssr.RiderParticipationId == ssr.StageSelection.KopmanId,
                            StagePos = rp.StagePos,
                            TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.StageScore * 0.5) : 0),
                            Selected = stageSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                            // TODO dnf
                        };
            var riderScores = query.OrderBy(r => r.StagePos).ToList();

            var gemistQuery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                              join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                              from rp in results.DefaultIfEmpty()
                              let totalScore = (budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0
                              where (ts.AccountParticipationId == user.AccountParticipationId) && (totalScore > 0 || allSelected.Contains(ts.RiderParticipationId))
                                && !DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.StageSelectionId == user.StageSelectionId).Any(ssr => ssr.RiderParticipationId == ts.RiderParticipationId)
                              select new StageComparisonRider
                              {
                                  Rider = ts.RiderParticipation.Rider,
                                  StagePos = rp.StagePos,
                                  TotalScore = totalScore,
                                  Selected = stageSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                                  // TODO dnf
                              };

            output.Add(new UserSelection(user.Username, riderScores, gemistQuery.ToList()));
        }

        var (teams, totals) = OrderSelectedRiders(output);
        return new(teams.OrderByDescending(x => x.Riders.Last().TotalScore), totals);
    }
}