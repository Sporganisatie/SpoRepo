using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StageController(DatabaseContext DB, IWebHostEnvironment env) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(StageStateEnum), 200)]
    public IActionResult Get(int raceId, int stageNr) => Ok(DB.ShowResults(raceId, stageNr) ? StageStateEnum.Started : StageStateEnum.Selection);

    [HttpGet("profiles")]
    public IActionResult Profiles(int raceId, int stageNr)
    {
        var race = DB.Races.AsNoTracking().SingleOrDefault(r => r.RaceId == raceId);
        if (race is null)
        {
            return NotFound();
        }

        var profilesRootPath = ResolveProfilesRootPath(env);
        var stageFolder = GetStageFolder(race, stageNr, profilesRootPath);
        var baseUrl = $"/profiles/{RaceString(race.Name)}/{race.Year}/stage-{stageNr}";
        if (!Directory.Exists(stageFolder))
        {
            return Ok(new { baseUrl, profile = string.Empty, climbs = Array.Empty<string>(), finishProfile = string.Empty });
        }

        var profile = Path.GetFileName(FindFile(stageFolder, "profile"));

        var finishProfile = Path.GetFileName(FindFile(stageFolder, "finish-profile"));

        var climbs = Directory.EnumerateFiles(stageFolder)
            .Select(file => (fileName: Path.GetFileName(file), nameWithoutExt: Path.GetFileNameWithoutExtension(file)))
            .Where(x => x.nameWithoutExt.StartsWith("climb-", StringComparison.OrdinalIgnoreCase))
            .Select(x => (x.fileName, index: int.TryParse(x.nameWithoutExt[6..], out var index) ? index : 0))
            .Where(x => x.index > 0)
            .OrderBy(x => x.index)
            .Select(x => x.fileName)
            .ToArray();

        return Ok(new { baseUrl, profile, climbs, finishProfile });
    }

    private static string GetStageFolder(Race race, int stageNr, string webRootPath)
        => Path.Combine(webRootPath, "profiles", RaceString(race.Name), race.Year.ToString(), $"stage-{stageNr}");

    private static string ResolveProfilesRootPath(IWebHostEnvironment env)
    {
        if (!string.IsNullOrWhiteSpace(env.WebRootPath))
        {
            return env.WebRootPath;
        }

        var contentRoot = string.IsNullOrWhiteSpace(env.ContentRootPath)
            ? Directory.GetCurrentDirectory()
            : env.ContentRootPath;
        return Path.Combine(contentRoot, "wwwroot");
    }

    private static string FindFile(string folder, string prefix)
        => Directory.EnumerateFiles(folder).FirstOrDefault(file => string.Equals(Path.GetFileNameWithoutExtension(file), prefix, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;

    private static string RaceString(string raceName)
        => raceName switch
        {
            "giro" => "giro-d-italia",
            "tour" => "tour-de-france",
            "vuelta" => "vuelta-a-espana",
            _ => throw new ArgumentOutOfRangeException()
        };
}
