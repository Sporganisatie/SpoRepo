using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StageController(DatabaseContext DB) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(StageStateEnum), 200)]
    public IActionResult Get(int raceId, int stageNr) => Ok(DB.ShowResults(raceId, stageNr) ? StageStateEnum.Started : StageStateEnum.Selection);
}
