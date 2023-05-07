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
    private readonly StageResultsClient StageResultsClient;
    public StageResultsController(StageResultsClient stageResultsClient)
    {
        StageResultsClient = stageResultsClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserScore), 200)]
    public IActionResult GetAccountStageResults(int raceId, bool budgetParticipation, int stagenr)
        => Ok(StageResultsClient.GetStageResultData(raceId, budgetParticipation, stagenr));
}
