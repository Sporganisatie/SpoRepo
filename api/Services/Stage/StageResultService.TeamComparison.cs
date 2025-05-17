using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Models.Response;
using static SpoRE.Helper.HelperFunctions;

namespace SpoRE.Services;

public partial class StageResultService
{
    public TeamSelections AllStageSelections(int raceId, bool budgetParticipation, int stagenr)
    {
        // TODO dit moet een stuk beter waarschijnlijk met wat server side evaluation
        if (DB.Stages.Single(s => s.RaceId == raceId && s.Stagenr == stagenr).Starttime > DateTime.UtcNow) return new([], []);

        var teamSelection = DB.AccountParticipations.Include(ap => ap.RiderParticipations).Single(ts => ts.AccountParticipationId == User.ParticipationId).RiderParticipations
            .Select(rp => rp.RiderParticipationId).ToList();
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var users = DB.StageSelections.Include(ss => ss.AccountParticipation.Account).Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation).Select(ss => new { ss.StageSelectionId, ss.AccountParticipation.Account.Username, ss.AccountParticipationId, ss.TotalScore }).ToList().OrderByDescending(x => x.TotalScore);
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

            var gemistQuery = DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider)
            .Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.ResultsPoints).AsNoTracking()
            .Where(ap => ap.AccountParticipationId == user.AccountParticipationId)
            .SelectMany(ap => ap.RiderParticipations).Where(ripa =>
                !DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.StageSelectionId == user.StageSelectionId).Any(ssr => ssr.RiderParticipationId == ripa.RiderParticipationId) &&
                (((budgetParticipation ? (ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Totalscore - ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Teamscore) : ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Totalscore) ?? 0) > 0 || allSelected.Contains(ripa.RiderParticipationId)))
                .Select(ripa => new StageComparisonRider
                {
                    Rider = ripa.Rider,
                    StagePos = ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).StagePos,
                    TotalScore = (budgetParticipation ? (ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Totalscore - ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Teamscore) : ripa.ResultsPoints.Single(rp => rp.Stage.Stagenr == stagenr).Totalscore) ?? 0,
                    Selected = stageSelection.Contains(ripa.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ripa.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                });

            output.Add(new UserSelection(user.Username, riderScores, gemistQuery.ToList()));
        }

        return OrderSelectedRiders(output);
    }
}