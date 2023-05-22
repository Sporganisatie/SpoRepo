using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChartsController : ControllerBase
{
    private StatisticsService Service;
    public ChartsController(StatisticsService service)
        => Service = service;

    [HttpGet("scoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult ScoreVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.ScoreVerloop(raceId, budgetParticipation));

    [HttpGet("positieVerloop")]
    [ProducesResponseType(200)]
    public IActionResult PositieVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.PositieVerloop(raceId, budgetParticipation));
}
