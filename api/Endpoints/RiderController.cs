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
    [ProducesResponseType(typeof(RaceState), 200)]
    public IActionResult GetAsync(int riderId)
        => Ok(Service.GetRiderInfo(riderId));
}
