
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class RaceService
{
    private readonly AccountClient AccountClient;
    private readonly RaceClient RaceClient;
    private readonly Userdata User;
    private readonly TeamSelectionClient TeamClient;

    public RaceService(AccountClient accountClient, RaceClient raceClient, Userdata userData, TeamSelectionClient databaseContext)
    {
        AccountClient = accountClient;
        RaceClient = raceClient;
        User = userData;
        TeamClient = databaseContext;
    }

    internal async Task<Result<RaceState>> GetRaceState(int raceId)
     => await AccountClient.GetParticipationCount(User.Id, raceId)
            .ActAsync(participationCount =>
            {
                var currStage = RaceClient.CurrentStage(raceId);
                if (currStage > 0)
                {
                    // get race has finished
                    return new RaceState(RaceStateEnum.Started, currStage);
                };
                return RaceState(participationCount);
            });

    private Result<RaceState> RaceState(int participationCount)
    {
        return new RaceState(participationCount > 0 ? RaceStateEnum.TeamSelection : RaceStateEnum.NotJoined, 0);
    }

    public Task<Result> JoinRace(int raceId)
        => TeamClient.JoinRace(raceId);
}