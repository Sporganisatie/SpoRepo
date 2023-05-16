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
    public async Task<IActionResult> GetAsync(int raceId)
        => Ok((await Service.GetRaceState(raceId)).Value);

    [HttpGet("join")]
    [ProducesResponseType(200)]
    [PreStart]
    public IActionResult JoinRace(int raceId)
        => Ok(Service.JoinRace(raceId));

    [HttpGet("comparison")]
    [ProducesResponseType(typeof(List<UserSelection>), 200)]
    [ParticipationEndpoint]
    public IActionResult StageSelections(int raceId, bool budgetParticipation)
        => Ok(StageService.AllTeamSelections(raceId, budgetParticipation));
}
