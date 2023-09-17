using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[CacheResponse]
public class ChartsController : ControllerBase
{
    private StatisticsService Service;
    public ChartsController(StatisticsService service)
        => Service = service;

    [HttpGet("scoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult ScoreVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.ScoreVerloop(raceId, budgetParticipation));

    [HttpGet("raceScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult RaceScoreVerloop(bool budgetParticipation)
        => Ok(Service.RaceScoreVerloop(budgetParticipation));

    [HttpGet("perfectScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult perfectScoreVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.PerfectScoreVerloop(raceId, budgetParticipation));

    [HttpGet("positieVerloop")]
    [ProducesResponseType(200)]
    public IActionResult PositieVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.PositieVerloop(raceId, budgetParticipation));

    [HttpGet("scoreSpread")]
    [ProducesResponseType(200)]
    public IActionResult ScoreSpread(int raceId, bool budgetParticipation)
        => Ok(Service.Uitslagen(raceId, budgetParticipation));
}
