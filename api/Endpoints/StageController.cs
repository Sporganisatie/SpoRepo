using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StageController : ControllerBase
{
    private readonly RaceClient _raceClient;

    public StageController(RaceClient raceClient)
        => _raceClient = raceClient;

    [HttpGet]
    [ProducesResponseType(typeof(StageStateEnum), 200)]
    public IActionResult Get(int raceId, int stageNr) => Ok(_raceClient.StageStarted(raceId, stageNr) ? StageStateEnum.Started : StageStateEnum.Selection);
}
