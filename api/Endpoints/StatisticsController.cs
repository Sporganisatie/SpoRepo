using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
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

    [HttpGet("missedPoints")]
    [ProducesResponseType(200)]
    [PreStart]
    public IActionResult JoinRace(int raceId)
        => Ok(Service.JoinRace(raceId));
}
