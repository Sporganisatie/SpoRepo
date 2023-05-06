using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ParticipationEndpoint(Order = 2)]
public class StageResultsController : ControllerBase
{
    private readonly TeamSelectionClient TeamSelectionClient;
    private readonly StageSelectionClient StageSelectionClient;
    public StageResultsController(TeamSelectionClient teamSelectionClient, StageSelectionClient stageSelectionClient)
    {
        TeamSelectionClient = teamSelectionClient;
        StageSelectionClient = stageSelectionClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StageSelectionData), 200)]
    public IActionResult GetAccountStageResults(int raceId, bool budgetParticipation, int stagenr)
        => Ok(StageSelectionClient.GetAccountStageResults(raceId, stagenr));
}
