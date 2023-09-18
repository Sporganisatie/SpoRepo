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
public class AdminController : ControllerBase
{
    private readonly Scrape Scraper;
    private readonly RaceService RaceService;
    private readonly Scheduler Scheduler;
    private readonly DatabaseContext DB;

    public AdminController(Scrape scrape, RaceService raceService, Scheduler scheduler, DatabaseContext database)
    {
        Scraper = scrape;
        RaceService = raceService;
        Scheduler = scheduler;
        DB = database;
    }

    [HttpGet("startlist")]
    public IActionResult ScrapeStartList(string raceName, int year)
    {
        Scraper.Startlist(raceName, year);
        return Ok();
    }

    [HttpGet("stageResults")]
    public async Task<IActionResult> GetAsync(string raceName, int year, int stagenr, bool mostRecent)
    {
        if (mostRecent)
        {
            await Scraper.StageResults(DB.MostRecentStartedStage());
            Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    [HttpGet("RaceFinished")]
    public IActionResult RaceFinished(int raceId)
        => Ok(RaceService.SetFinished(raceId));
}
