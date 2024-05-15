using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaceController(RaceService Service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RaceState), 200)]
    public IActionResult GetAsync(int raceId)
        => Ok(Service.GetRaceState(raceId));

    [HttpGet("join")]
    [ProducesResponseType(200)]
    [PreStart]
    public IActionResult JoinRace(int raceId)
        => Ok(Service.JoinRace(raceId));

    [HttpGet("comparison")]
    [ProducesResponseType(typeof((IEnumerable<UserSelection>, IEnumerable<(Rider, int)>)), 200)]
    [PostStart]
    [ParticipationEndpoint]
    // [CacheResponse] TODO introduce way to identify user for cache key
    public IActionResult AllTeamSelections(int raceId, bool budgetParticipation)
        => Ok(Service.AllTeamSelections(raceId, budgetParticipation));
}
