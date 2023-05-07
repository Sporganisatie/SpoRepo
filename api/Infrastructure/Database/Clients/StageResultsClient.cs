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
        var teamResult = GetTeamResult(stagenr);
        return new(userScores, teamResult);
    }

    private IEnumerable<ResultsPoint> GetTeamResult(int stagenr)
    {
        var opgesteldeRenners = from ssr in DB.StageSelectionRiders
                                where ssr.StageSelection.Stage.Stagenr == stagenr &&
                                      ssr.StageSelection.AccountParticipationId == User.ParticipationId
                                select ssr;

        var result = from rp in DB.ResultsPoints
                     where rp.Stage.Stagenr == stagenr &&
                           opgesteldeRenners.Any(ssr => ssr.RiderParticipationId == rp.RiderParticipationId)
                     select rp;
        return result.ToList();
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
