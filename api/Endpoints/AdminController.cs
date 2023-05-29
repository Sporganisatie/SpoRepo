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
    private readonly RaceClient RaceClient;
    private readonly RaceService RaceService;
    private readonly Scheduler Scheduler;
    public AdminController(Scrape scrape, RaceClient raceClient, RaceService raceService, Scheduler scheduler)
    {
        Scraper = scrape;
        RaceService = raceService;
        RaceClient = raceClient;
        Scheduler = scheduler;
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
            await Scraper.StageResults(RaceClient.MostRecentStartedStage());
            Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    [HttpGet("RaceFinished")]
    public IActionResult RaceFinished(int raceId)
        => Ok(RaceService.SetFinished(raceId));
}
