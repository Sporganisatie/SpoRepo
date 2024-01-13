using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaceController : ControllerBase
{
    private RaceService Service;
    private readonly StageResultService StageService;

    public RaceController(RaceService service, StageResultService stageResultService)

    {
        Service = service;
        StageService = stageResultService;
    }

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
    [ProducesResponseType(typeof(List<UserSelection>), 200)]
    [PostStart]
    [ParticipationEndpoint]
    // [CacheResponse] TODO introduce way to identify user for cache key
    public IActionResult AllTeamSelections(int raceId, bool budgetParticipation)
        => Ok(StageService.AllTeamSelections(raceId, budgetParticipation));
}
