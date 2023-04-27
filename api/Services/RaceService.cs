
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;

namespace SpoRE.Services;

public class RaceService
{
    private readonly AccountClient _accountClient;
    private readonly Userdata User;

    public RaceService(AccountClient accountClient, Userdata userData)
    {
        _accountClient = accountClient;
        User = userData;
    }

    internal async Task<Result<RaceStateEnum>> GetRaceState(int raceId)
     => await _accountClient.GetParticipationCount(User.Id, raceId) // TODO ook kijken of race gestart is
            .ActAsync(participationCount => RaceState(participationCount));

    private Result<RaceStateEnum> RaceState(int participationCount)
    {
        return participationCount > 0 ? RaceStateEnum.TeamSelection : RaceStateEnum.NotJoined;
    }
}