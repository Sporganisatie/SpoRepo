using SpoRE.Infrastructure.Database.Stage;

namespace SpoRE.Services;

public class StageService
{
    private StageClient _stageClient;

    public StageService(StageClient stageClient)
    {
        _stageClient = stageClient;
    }
    public async Task<object> TeamResults(int raceId, int stagenr, bool budgetParticipation)
    {
        // TODO mappen van TeamResultRow naar een FE model met Rider 
        return await _stageClient.TeamResults(raceId, stagenr, budgetParticipation);
    }
}