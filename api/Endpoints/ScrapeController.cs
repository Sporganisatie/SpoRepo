using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Scrape;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Admin]
public class ScrapeController : ControllerBase
{
    private readonly Scrape Scraper;
    public ScrapeController(Scrape scrape)
    {
        Scraper = scrape;
    }

    [HttpGet("startlist")]
    public IActionResult Get(string raceName, int year)
    {
        Scraper.Startlist(raceName, year);
        return Ok();
    }

    [HttpGet("stageResults")]
    public IActionResult Get(string raceName, int year, int stagenr)
    {
        Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }
}
