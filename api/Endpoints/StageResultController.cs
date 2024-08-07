using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Helper;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ParticipationEndpoint]
// [CacheResponse] TODO introduce way to identify user for cache key
public class StageResultController(StageResultService service) : ControllerBase
{
    private readonly StageResultService Service = service;
    [HttpGet]
    [ProducesResponseType(typeof(UserScore), 200)]
    public IActionResult StageResultData(int raceId, bool budgetParticipation, int stagenr)
        => Ok(Service.StageResultData(raceId, budgetParticipation, stagenr));

    [HttpGet("comparison")]
    [ProducesResponseType(typeof(TeamSelections), 200)]
    public IActionResult StageSelections(int raceId, bool budgetParticipation, int stagenr)
        => Ok(Service.AllStageSelections(raceId, budgetParticipation, stagenr));
}
