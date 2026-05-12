using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Infrastructure.Scrape;
using SpoRE.Services;
using SpoRE.Setup;

namespace SpoRE.Controllers;

public class AdminExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        context.Result = new ObjectResult(new
        {
            message = ex.Message,
            type = ex.GetType().FullName,
            stackTrace = ex.StackTrace,
            inner = ex.InnerException?.ToString()
        })
        { StatusCode = 500 };
        context.ExceptionHandled = true;
    }
}

[ApiController]
[Route("api/[controller]")]
[Admin]
[AdminExceptionFilter]
public class AdminController(Scrape Scraper, RaceService RaceService, Scheduler Scheduler, DatabaseContext DB, IMemoryCache MemoryCache) : ControllerBase
{
    [HttpGet("startlist")]
    public async Task<IActionResult> ScrapeStartList(string raceName, int year, int raceId)
    {
        await Scraper.Startlist(raceName, year, raceId);
        return Ok();
    }

    [HttpGet("stageResults")]
    public async Task<IActionResult> GetAsync(string raceName, int year, int stagenr, bool mostRecentStarted, bool aankomende)
    {
        if (mostRecentStarted)
        {
            var mostRecentStartedStage = DB.Stages.Include(s => s.Race).OrderByDescending(s => s.Starttime).ToList().First(s => s.Starttime < DateTime.UtcNow);
            await Scraper.StageResults(mostRecentStartedStage);
            await Scheduler.RunTimer();
        }
        else if (aankomende)
        {
            var aankomendeEtappe = DB.Stages.Include(s => s.Race).OrderBy(s => s.Starttime).ToList().First(s => !s.Complete);
            await Scraper.StageResults(aankomendeEtappe);
            await Scheduler.RunTimer();
        }
        else await Scraper.StageResults(raceName, year, stagenr);
        return Ok();
    }

    [HttpGet("RaceFinished")]
    public async Task<IActionResult> RaceFinished(int raceId)
        => Ok(await RaceService.SetFinished(raceId));

    [HttpGet("AddStages")]
    public async Task<IActionResult> AddStages(int raceId)
        => Ok(await Scraper.EtappesToevoegen(raceId));

    [HttpGet("DownloadStageProfiles")]
    public async Task<IActionResult> DownloadStageProfiles(int raceId)
    {
        await Scraper.DownloadStageProfiles(raceId);
        return Ok();
    }

    [HttpGet("resetCache")]
    public IActionResult ResetCache()
    {
        ((MemoryCache)MemoryCache).Clear();
        return Ok("Cache has been reset.");
    }

    [HttpGet("playwrightDiag")]
    public IActionResult PlaywrightDiag()
    {
        string? Listing(string? dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) return null;
            try { return string.Join("\n", Directory.EnumerateFileSystemEntries(dir).Take(50)); }
            catch (Exception e) { return $"<error: {e.Message}>"; }
        }

        var asmDir = Path.GetDirectoryName(typeof(AdminController).Assembly.Location);
        var baseDir = AppContext.BaseDirectory;
        var procDir = Path.GetDirectoryName(Environment.ProcessPath);
        var cwd = Environment.CurrentDirectory;

        return Ok(new
        {
            assemblyLocation = typeof(AdminController).Assembly.Location,
            assemblyDir = asmDir,
            appContextBaseDir = baseDir,
            processPath = Environment.ProcessPath,
            processDir = procDir,
            currentDirectory = cwd,
            home = Environment.GetEnvironmentVariable("HOME"),
            playwrightDriverPath = Environment.GetEnvironmentVariable("PLAYWRIGHT_DRIVER_PATH"),
            playwrightBrowsersPath = Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH"),
            assemblyDirListing = Listing(asmDir),
            baseDirListing = Listing(baseDir),
            procDirListing = Listing(procDir),
            cwdListing = Listing(cwd),
            playwrightInAssembly = asmDir != null && Directory.Exists(Path.Combine(asmDir, ".playwright")),
            playwrightInBase = Directory.Exists(Path.Combine(baseDir, ".playwright")),
        });
    }
}
