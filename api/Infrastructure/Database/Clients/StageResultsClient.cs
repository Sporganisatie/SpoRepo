using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Models.Response;
using Z.EntityFramework.Plus;

namespace SpoRE.Infrastructure.Database;

public class StageResultsClient
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;
    public StageResultsClient(DatabaseContext databaseContext, Userdata userData)
    {
        DB = databaseContext;
        User = userData;
    }

    public StageResultData GetStageResultData(int raceId, bool budgetParticipation, int stagenr)
    {
        var userScores = GetUserScores(raceId, budgetParticipation, stagenr);
        var teamResult = GetTeamResult(raceId, stagenr, budgetParticipation);
        return new(userScores, teamResult);
    }

    private IEnumerable<RiderScore> GetTeamResult(int raceId, int stagenr, bool budgetParticipation)
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
        var query = from ssr in DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.Stage.Stagenr == stagenr && ssr.StageSelection.AccountParticipationId == User.ParticipationId)
                    join rp in DB.ResultsPoints.Include(rp => rp.RiderParticipation.Rider) on ssr.RiderParticipationId equals rp.RiderParticipationId
                    where rp.Stage.Stagenr == stagenr && rp.Stage.RaceId == raceId
                    select new RiderScore
                    {
                        Rider = rp.RiderParticipation.Rider,
                        StagePos = rp.Stagepos,
                        StageScore = (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 1.5) : rp.Stagescore) ?? 0,
                        ClassificationScore = rp.Gcscore + rp.Pointsscore + rp.Komscore + rp.Yocscore ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 0.5) : 0)
                    };
        return query.ToList().OrderByDescending(rc => rc.TotalScore).ThenBy(rc => rc.StagePos);
    }

    private IEnumerable<UserScore> GetUserScores(int raceId, bool budgetParticipation, int stagenr)
        => DB.StageSelections.Where(ss => ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr)
            .Join(
                DB.AccountParticipations.Where(ap => ap.Budgetparticipation == budgetParticipation),
                ss => ss.AccountParticipationId,
                ap => ap.AccountParticipationId,
                (ss, ap) => new UserScore(ap.Account, ss.Stagescore, ss.Totalscore)
            ).ToList().OrderByDescending(us => us.totalscore).ThenByDescending(us => us.stagescore);
}
