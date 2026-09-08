using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[CacheResponse]
public class ChartsController(StatisticsService Service) : ControllerBase
{
    [HttpGet("scoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult ScoreVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.ScoreVerloop(raceId, budgetParticipation));

    [HttpGet("raceScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult RaceScoreVerloop(bool budgetParticipation)
        => Ok(Service.RaceScoreVerloop(budgetParticipation));

    [HttpGet("stagenummerScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult StagenummerScoreVerloop(bool budgetParticipation, bool genormaliseerd, bool totaalScore = false)
        => Ok(Service.StagenummerScoreVerloop(budgetParticipation, genormaliseerd, totaalScore));

    [HttpGet("racePositieScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult RacePositieScoreVerloop(bool budgetParticipation, bool bigFour, bool relative, int startRaceId, int endRaceId, PositionScoringMethod puntentelling)
        => Ok(Service.RacePositieScoreVerloop(budgetParticipation, bigFour, relative, startRaceId, endRaceId, puntentelling));

    [HttpGet("racePositieScoreRaceOptions")]
    [ProducesResponseType(200)]
    public IActionResult RacePositieScoreRaceOptions()
        => Ok(Service.RacePositieScoreRaceOptions());

    [HttpGet("perfectScoreVerloop")]
    [ProducesResponseType(200)]
    public IActionResult PerfectScoreVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.PerfectScoreVerloop(raceId, budgetParticipation));

    [HttpGet("positieVerloop")]
    [ProducesResponseType(200)]
    public IActionResult PositieVerloop(int raceId, bool budgetParticipation)
        => Ok(Service.PositieVerloop(raceId, budgetParticipation));

    [HttpGet("scoreSpread")]
    [ProducesResponseType(200)]
    public IActionResult ScoreSpread(int raceId, bool budgetParticipation)
        => Ok(Service.Uitslagen(raceId, budgetParticipation));
}
