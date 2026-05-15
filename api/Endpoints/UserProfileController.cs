using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfileController(UserProfileService Service, RaceService RaceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileData), 200)]
    [ProducesResponseType(404)]
    public IActionResult Get(string username, bool budgetParticipation, int raceId)
    {
        var selectedRaceId = raceId == 0 ? RaceService.Current() : raceId;

        var data = Service.GetProfile(username, budgetParticipation, selectedRaceId);
        if (data is null)
        {
            return NotFound();
        }

        return Ok(data);
    }
}
