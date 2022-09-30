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
    public async Task<object> TeamResults(int raceId, int stagenr, [FromQuery] bool? budgetParticipation)
    { // TODO return type List<something>, kopman
        return await _stageService.TeamResults(raceId, stagenr, budgetParticipation ?? false);
    }
}
