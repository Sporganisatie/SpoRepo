using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ParticipationEndpoint]
public class TeamSelectionController : ControllerBase
{
    private TeamSelectionService Service;
    public TeamSelectionController(TeamSelectionService service)
        => Service = service;

    [HttpGet()]
    [ProducesResponseType(typeof(TeamSelectionData), 200)]
    public IActionResult Get(int raceId, bool budgetParticipation)
        => Ok(Service.GetTeamSelectionData(raceId, budgetParticipation));

    [HttpPost()]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Add(int riderParticipationId, int raceId, bool budgetParticipation)
        => Ok(Service.AddRider(riderParticipationId, raceId, budgetParticipation));

    [HttpDelete()]
    [ProducesResponseType(typeof(int), 200)]
    public IActionResult Remove(int riderParticipationId, int raceId, bool budgetParticipation)
        => Ok(Service.RemoveRider(riderParticipationId, raceId, budgetParticipation));
}
