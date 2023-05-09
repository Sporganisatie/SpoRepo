using Microsoft.EntityFrameworkCore;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    public IEnumerable<RiderScore> GetTeamResult(int raceId, int stagenr, bool budgetParticipation)
    {
        var riderScores = GetRiderScores(raceId, stagenr, budgetParticipation).ToList();

        var totals = new RiderScore
        {
            StageScore = riderScores.Sum(rs => rs.StageScore),
            ClassificationScore = riderScores.Sum(rs => rs.ClassificationScore),
            TeamScore = riderScores.Sum(rs => rs.TeamScore),
            TotalScore = riderScores.Sum(rs => rs.TotalScore)
        };
        return riderScores.Append(totals);
    }

    private IEnumerable<RiderScore> GetRiderScores(int raceId, int stagenr, bool budgetParticipation)
    {
        var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                    join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                    from rp in results.DefaultIfEmpty()
                    where ssr.StageSelection.Stage.Stagenr == stagenr && ssr.StageSelection.AccountParticipationId == User.ParticipationId
                    select new RiderScore
                    {
                        Rider = ssr.RiderParticipation.Rider,
                        Kopman = ssr.RiderParticipationId == ssr.StageSelection.KopmanId,
                        StagePos = rp.Stagepos,
                        StageScore = (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 1.5) : rp.Stagescore) ?? 0,
                        ClassificationScore = rp.Gcscore + rp.Pointsscore + rp.Komscore + rp.Yocscore ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 0.5) : 0)
                    };

        return query.ToList().OrderByDescending(rc => rc.TotalScore).ThenBy(rc => rc.StagePos);
    }

    public IEnumerable<UserScore> GetUserScores(int raceId, bool budgetParticipation, int stagenr)
        => DB.StageSelections.Where(ss => ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr)
            .Join(
                DB.AccountParticipations.Where(ap => ap.Budgetparticipation == budgetParticipation),
                ss => ss.AccountParticipationId,
                ap => ap.AccountParticipationId,
                (ss, ap) => new UserScore(ap.Account, ss.Stagescore ?? 0, ss.Totalscore ?? 0)
            ).ToList().OrderByDescending(us => us.totalscore).ThenByDescending(us => us.stagescore);
}