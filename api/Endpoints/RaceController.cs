using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Models.Response;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaceController : ControllerBase
{
    private RaceService Service;
    public RaceController(RaceService service)
        => Service = service;

    [HttpGet]
    [ProducesResponseType(typeof(RaceState), 200)]
    public async Task<IActionResult> GetAsync(int raceId)
        => Ok((await Service.GetRaceState(raceId)).Value);

    [HttpGet("join")]
    [ProducesResponseType(200)]
    [PreStart]
    public IActionResult JoinRace(int raceId)
        => Ok(Service.JoinRace(raceId));
}
