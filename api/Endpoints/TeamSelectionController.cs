using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamSelectionController : ControllerBase
{
    private TeamSelectionService Service;
    public TeamSelectionController(TeamSelectionService service)
        => Service = service;

    [HttpGet()]
    [ProducesResponseType(200, Type = typeof(TeamSelectionData))]
    public IActionResult Get(int raceId, bool budgetParticipation)
        => Ok(Service.GetTeamSelectionData(raceId, budgetParticipation));
}
