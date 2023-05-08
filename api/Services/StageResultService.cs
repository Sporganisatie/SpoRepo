
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
        var userScores = GetUserScores(raceId, budgetParticipation, stagenr);
        var teamResult = GetTeamResult(raceId, stagenr, budgetParticipation);
        return new(userScores, teamResult);
    }

    public int AllStageSelections(int raceId, bool budgetParticipation, int stagenr)
    {
        var selections = DB.StageSelections.Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.Budgetparticipation == budgetParticipation);

        foreach (var selection in selections)
        {
            // copy paste get team hmm misschien code aanpassen?
        }
        return 0;
    }
}