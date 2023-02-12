using SpoRE.Infrastructure.Database.Teamselection;

namespace SpoRE.Services;

public class TeamselectionService
{
    private TeamselectionClient _teamselectionClient;

    public TeamselectionService(TeamselectionClient teamselectionClient)
    {
        _teamselectionClient = teamselectionClient;
    }

    public Task<Result<List<SpoRE.Infrastructure.Database.Teamselection.RiderParticipationRider>>> Get(int raceId)
        => _teamselectionClient.Get(raceId);
}
