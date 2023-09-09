using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private StatisticsService Service;
    public StatisticsController(StatisticsService service)
        => Service = service;

    [HttpGet("raceUitslagen")]
    [ProducesResponseType(200)]
    public IActionResult RaceUitslagen(bool budgetParticipation)
        => Ok(Service.RaceUitslagenAll(budgetParticipation));

    [HttpGet("missedPoints")]
    [ProducesResponseType(200)]
    [PostStart]
    public IActionResult MissedPoints(int raceId, bool budgetParticipation)
        => Ok(Service.MissedPoints(raceId, budgetParticipation));

    [HttpGet("uitvallers")]
    [ProducesResponseType(200)]
    [PostStart]
    public IActionResult Uitvallers(int raceId, bool budgetParticipation)
        => Ok(Service.Uitvallers(raceId, budgetParticipation));

    [HttpGet("etappeUitslagen")]
    [ProducesResponseType(200)]
    [PostStart]
    public IActionResult EtappeUitslagen(int raceId, bool budgetParticipation)
        => Ok(Service.EtappeUitslagen(raceId, budgetParticipation));

    [HttpGet("allRiders")]
    [ProducesResponseType(200)]
    [PostStart]
    public IActionResult AllRiders(int raceId, bool budgetParticipation)
        => Ok(Service.AllRiders(raceId, budgetParticipation));

    [HttpGet("klassementen")]
    [ProducesResponseType(200)]
    [PostStart]
    public IActionResult Klassementen(int raceId, bool budgetParticipation)
        => Ok(Service.Klassementen(raceId, budgetParticipation));
}
