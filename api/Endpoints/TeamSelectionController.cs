using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamSelectionController : ControllerBase
{
    private TeamSelectionClient Client;
    public TeamSelectionController(TeamSelectionClient client)
    {
        Client = client;
    }

    [HttpGet("team")]
    public ActionResult<IEnumerable<RiderParticipation>> GetTeam(int raceId, bool budgetParticipation)
    {
        // Todo auto attach user
        return Ok(Client.GetTeam(2, raceId, budgetParticipation));
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<RiderParticipation>> GetAll(int raceId, bool budgetParticipation)
    {
        // Todo auto attach user
        return Ok(Client.GetAll(2, raceId, budgetParticipation));
    }
}
