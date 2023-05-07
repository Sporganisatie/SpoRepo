using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Infrastructure.Scrape;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Admin]
public class AdminController : ControllerBase
{
    private readonly Scrape Scraper;
    private readonly StageClient StageClient;
    public AdminController(Scrape scrape, StageClient stageClient)
    {
        Scraper = scrape;
        StageClient = stageClient;
    }

    [HttpGet("startlist")]
    public IActionResult Get(string raceName, int year)
    {
        Scraper.Startlist(raceName, year);
        return Ok();
    }

    [HttpGet("stageResults")]
    public IActionResult Get(string raceName, int year, int stagenr, bool mostRecent)
    {
        if (mostRecent) Scraper.StageResults(StageClient.MostRecentStartedStage());
        else Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    // [HttpGet("stageResults")]
    // public IActionResult Get()
    // {
    //     // Scraper.StageResults(StageClient.MostRecentStartedStage());
    //     return Ok();
    // }
}
