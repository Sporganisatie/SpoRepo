using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services.StageSelection;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[PreStart(Order = 1)]
[ParticipationEndpoint(Order = 2)]
public class StageSelectionController(StageSelectionService StageSelectionClient) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(StageSelectionData), 200)]
    public IActionResult Get(int raceId, int stagenr)
        => Ok(StageSelectionClient.GetData(raceId, stagenr));

    [HttpPost("rider")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Add(int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.AddRider(riderParticipationId, stagenr));

    [HttpDelete("rider")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Remove(int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.RemoveRider(riderParticipationId, stagenr));

    [HttpPost("kopman")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult AddKopman(int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.SetKopman(riderParticipationId, stagenr));

    [HttpDelete("kopman")]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult RemoveKopman(int riderParticipationId, int stagenr)
        => Ok(StageSelectionClient.RemoveKopman(riderParticipationId, stagenr));
}
