using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Infrastructure.Scrape;
using SpoRE.Setup;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Admin]
public class AdminController : ControllerBase
{
    private readonly Scrape Scraper;
    private readonly StageClient StageClient;
    private readonly Scheduler Scheduler;
    public AdminController(Scrape scrape, StageClient stageClient, Scheduler scheduler)
    {
        Scraper = scrape;
        StageClient = stageClient;
        Scheduler = scheduler;
    }

    [HttpGet("startlist")]
    public IActionResult Get(string raceName, int year)
    {
        Scraper.Startlist(raceName, year);
        return Ok();
    }

    [HttpGet("stageResults")]
    public async Task<IActionResult> GetAsync(string raceName, int year, int stagenr, bool mostRecent)
    {
        if (mostRecent)
        {
            await Scraper.StageResults(StageClient.MostRecentStartedStage());
            Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }
}
