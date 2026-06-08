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
public class AdminController(
    Scrape Scraper,
    RaceService RaceService,
    Scheduler Scheduler,
    DatabaseContext DB,
    IMemoryCache MemoryCache,
    StageSelectionStatsService StageSelectionStatsService,
    AccountParticipationStatsService AccountParticipationStatsService) : ControllerBase
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
            var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var mostRecentStartedStage = await DB.Stages.Include(s => s.Race).OrderByDescending(s => s.Starttime).FirstAsync(s => s.Starttime < now);
            await Scraper.StageResults(mostRecentStartedStage);
            await Scheduler.RunTimer();
        }
        else if (aankomende)
        {
            var aankomendeEtappe = await DB.Stages.Include(s => s.Race).OrderBy(s => s.Starttime).FirstOrDefaultAsync(s => !s.Complete && !s.Race.Finished);
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

    [HttpGet("CalculateStageSelectionStats")]
    public async Task<IActionResult> CalculateStageSelectionStats(int raceId, int? stagenr)
    {
        if (stagenr.HasValue)
        {
            var stage = await DB.Stages
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.RaceId == raceId && s.Stagenr == stagenr.Value);

            if (stage == null)
            {
                return NotFound($"No stage found for raceId={raceId} and stagenr={stagenr.Value}");
            }

            await StageSelectionStatsService.Calculate(stage.StageId);
            return Ok(new { raceId, stage.Stagenr, calculated = 1 });
        }

        var finishedStages = await DB.Stages
            .AsNoTracking()
            .Where(s => s.RaceId == raceId && s.Finished)
            .OrderBy(s => s.Stagenr)
            .Select(s => new { s.StageId, s.Stagenr })
            .ToListAsync();

        foreach (var stage in finishedStages)
        {
            await StageSelectionStatsService.Calculate(stage.StageId);
        }

        return Ok(new
        {
            raceId,
            calculated = finishedStages.Count,
            finishedStages = finishedStages.Select(s => s.Stagenr).ToList()
        });
    }

    [HttpGet("CalculateAccountParticipationStats")]
    public async Task<IActionResult> CalculateAccountParticipationStats(int raceId)
    {
        await AccountParticipationStatsService.Calculate(raceId);
        return Ok(new { raceId });
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
