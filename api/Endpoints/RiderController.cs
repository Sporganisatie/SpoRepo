using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RiderController(RiderService Service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RiderOverview), 200)]
    public IActionResult GetAsync(int riderId, bool budgetParticipation)
        => Ok(Service.GetRiderOverview(riderId, budgetParticipation));

    [HttpGet("race")]
    [ProducesResponseType(typeof(RiderRaceDetail), 200)]
    public IActionResult GetRaceDetail(int riderId, int raceId, bool budgetParticipation)
        => Ok(Service.GetRiderRaceDetail(riderId, raceId, budgetParticipation));
}
