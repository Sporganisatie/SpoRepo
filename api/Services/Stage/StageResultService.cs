using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService(DatabaseContext DB, Userdata User)
{
    public StageResultData StageResultData(int raceId, bool budgetParticipation, int stagenr)
    {
        if (!DB.ShowResults(raceId, stagenr)) return new([], [], Classifications.Empty);
        var stage = DB.Stages.Single(ss => ss.RaceId == raceId && ss.Stagenr == stagenr);
        var userScores = GetUserScores(stage, budgetParticipation);
        var teamResult = GetTeamResult(stage, budgetParticipation);
        var classifications = GetClassifications(stage, top5: false);
        return new(userScores, teamResult, classifications);
    }
}