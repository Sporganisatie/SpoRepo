
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;

namespace SpoRE.Services;

public class RaceService
{
    private readonly AccountClient _accountClient;
    private readonly RaceClient _raceClient;
    private readonly Userdata User;
    private readonly TeamSelectionClient TeamClient;

    public RaceService(AccountClient accountClient, RaceClient raceClient, Userdata userData, TeamSelectionClient databaseContext)
    {
        _accountClient = accountClient;
        _raceClient = raceClient;
        User = userData;
        TeamClient = databaseContext;
    }

    internal async Task<Result<RaceStateEnum>> GetRaceState(int raceId)
     => await _accountClient.GetParticipationCount(User.Id, raceId) // TODO ook kijken of race gestart is
            .ActAsync(participationCount =>
            {
                // get race has finished
                if (_raceClient.StageStarted(raceId, 1))
                {
                    return RaceStateEnum.Started;
                };
                return RaceState(participationCount);
            });

    private Result<RaceStateEnum> RaceState(int participationCount)
    {
        return participationCount > 0 ? RaceStateEnum.TeamSelection : RaceStateEnum.NotJoined;
    }

    public Task<Result> JoinRace(int raceId)
        => TeamClient.JoinRace(raceId);
}