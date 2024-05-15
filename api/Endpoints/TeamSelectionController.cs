using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[PreStart(Order = 1)]
[ParticipationEndpoint(Order = 2)]
public class TeamSelectionController(TeamSelectionService Service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(TeamSelectionData), 200)]
    public IActionResult Get(int raceId, bool budgetParticipation)
        => Ok(Service.GetTeamSelectionData(raceId, budgetParticipation));

    [HttpPost]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Add(int riderParticipationId, int raceId, bool budgetParticipation)
        => Ok(Service.AddRider(riderParticipationId, raceId, budgetParticipation));

    [HttpDelete]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Remove(int riderParticipationId)
        => Ok(Service.RemoveRider(riderParticipationId));
}
