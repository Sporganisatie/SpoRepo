using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaceWrapController(RaceWrapService Service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult RaceScoreStatistics(bool budgetParticipation)
        => Ok(Service.RaceScoreStatistics(budgetParticipation));
}
