using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamselectionController
{
    public TeamselectionService _teamselectionService;
    public TeamselectionController(TeamselectionService teamselectionService)
    {
        _teamselectionService = teamselectionService;
    }

    [HttpGet("{raceId}/rider_participation")]
    public async Task<Result<List<SpoRE.Infrastructure.Database.Teamselection.RiderParticipationRider>>> Get(int raceId)
        => await _teamselectionService.Get(raceId); // TODO niet return result (en misschien niet eens task)
}
