using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StageController
{
    public StageService _stageService;
    public StageController(StageService stageService)
    {
        _stageService = stageService;
    }

    [HttpGet("{raceId}/{stagenr}/teamresults")]
    public async Task<Result<IEnumerable<TeamResultRow>>> TeamResults(int raceId, int stagenr, [FromQuery] bool? budgetParticipation)
    { // TODO niet return result (en misschien niet eens task)
        return await _stageService.TeamResults(raceId, stagenr, budgetParticipation ?? false);
    }
}
