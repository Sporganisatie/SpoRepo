using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;

    public StageResultService(DatabaseContext databaseContext, Userdata userData)
    {
        DB = databaseContext;
        User = userData;
    }

    public StageResultData StageResultData(int raceId, bool budgetParticipation, int stagenr)
    {
        if (!DB.ShowResults(raceId, stagenr)) return new(new List<UserScore>(), new List<RiderScore>(), Classifications.Empty);
        var stage = DB.Stages.Single(ss => ss.RaceId == raceId && ss.Stagenr == stagenr);
        var userScores = GetUserScores(stage, budgetParticipation);
        var teamResult = GetTeamResult(stage, budgetParticipation);
        var classifications = GetClassifications(stage, top5: false);
        return new(userScores, teamResult, classifications);
    }
}