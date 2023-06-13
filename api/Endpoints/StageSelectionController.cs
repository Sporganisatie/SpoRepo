using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services.StageSelection;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[PreStart(Order = 1)]
[ParticipationEndpoint(Order = 2)]
public class StageSelectionController : ControllerBase
{
    private readonly StageSelectionService StageSelectionClient;

    public StageSelectionController(StageSelectionService stageSelectionClient)
    {
        StageSelectionClient = stageSelectionClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StageSelectionData), 200)]
    public IActionResult Get(int raceId, bool budgetParticipation, int stagenr)
        => Ok(StageSelectionClient.GetData(raceId, stagenr));

    [HttpPost("rider")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Add(int raceId, bool budgetParticipation, int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.AddRider(riderParticipationId, stagenr));

    [HttpDelete("rider")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Remove(int raceId, bool budgetParticipation, int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.RemoveRider(riderParticipationId, stagenr));

    [HttpPost("kopman")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult AddKopman(int raceId, bool budgetParticipation, int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.SetKopman(riderParticipationId, stagenr));

    [HttpDelete("kopman")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult RemoveKopman(int raceId, bool budgetParticipation, int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.RemoveKopman(riderParticipationId, stagenr));
}
