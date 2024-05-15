using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Infrastructure.Scrape;
using SpoRE.Services;
using SpoRE.Setup;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Admin]
public class AdminController(Scrape Scraper, RaceService RaceService, Scheduler Scheduler, DatabaseContext DB) : ControllerBase
{
    [HttpGet("startlist")]
    public IActionResult ScrapeStartList(string raceName, int year)
    {
        Scraper.Startlist(raceName, year);
        return Ok();
    }

    [HttpGet("stageResults")]
    public async Task<IActionResult> GetAsync(string raceName, int year, int stagenr, bool mostRecentStarted, bool aankomende)
    {
        if (mostRecentStarted)
        {
            await Scraper.StageResults(DB.MostRecentStartedStage());
            Scheduler.RunTimer();
        }
        else if (aankomende)
        {
            await Scraper.StageResults(DB.Aankomende());
            Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    [HttpGet("RaceFinished")]
    public IActionResult RaceFinished(int raceId)
        => Ok(RaceService.SetFinished(raceId));
}
