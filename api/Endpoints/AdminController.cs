using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public IActionResult ScrapeStartList(string raceName, int year, int raceId)
    {
        Scraper.Startlist(raceName, year, raceId);
        return Ok();
    }

    [HttpGet("stageResults")]
    public async Task<IActionResult> GetAsync(string raceName, int year, int stagenr, bool mostRecentStarted, bool aankomende)
    {
        if (mostRecentStarted)
        {
            var mostRecentStartedStage = DB.Stages.Include(s => s.Race).OrderByDescending(s => s.Starttime).ToList().First(s => s.Starttime < DateTime.UtcNow);
            await Scraper.StageResults(mostRecentStartedStage);
            Scheduler.RunTimer();
        }
        else if (aankomende)
        {
            var aankomendeEtappe = DB.Stages.Include(s => s.Race).OrderBy(s => s.Starttime).ToList().First(s => !s.Complete);
            await Scraper.StageResults(aankomendeEtappe);
            Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    [HttpGet("RaceFinished")]
    public IActionResult RaceFinished(int raceId)
        => Ok(RaceService.SetFinished(raceId));

    [HttpGet("AddStages")]
    public IActionResult AddStages(int raceId)
        => Ok(Scraper.EtappesToevoegen(raceId));
}
