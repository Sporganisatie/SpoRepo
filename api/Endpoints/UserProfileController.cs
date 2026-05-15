using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfileController(UserProfileService Service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileData), 200)]
    [ProducesResponseType(404)]
    public IActionResult Get(string username, bool budgetParticipation)
    {
        var data = Service.GetProfile(username, budgetParticipation);
        if (data is null)
        {
            return NotFound();
        }

        return Ok(data);
    }
}
