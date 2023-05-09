using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ParticipationEndpoint(Order = 2)]
public class StageResultController : ControllerBase
{
    private readonly StageResultService Service;
    public StageResultController(StageResultService service)
    {
        Service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserScore), 200)]
    public IActionResult AccountStageResults(int raceId, bool budgetParticipation, int stagenr)
        => Ok(Service.StageResultData(raceId, budgetParticipation, stagenr));

    [HttpGet("comparison")]
    [ProducesResponseType(typeof(List<UserSelection>), 200)]
    public IActionResult StageSelections(int raceId, bool budgetParticipation, int stagenr)
        => Ok(Service.AllStageSelections(raceId, budgetParticipation, stagenr));
}
