using System.Text.Json;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Services;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape(DatabaseContext DB, IMemoryCache MemoryCache, StageSelectionStatsService StageSelectionStatsService)
{
    private const string PcsStage = "STAGE";
    private const string PcsGc = "GC";
    private const string PcsPoints = "POINTS";
    private const string PcsKom = "KOM";
    private const string PcsYouth = "YOUTH";
    private const string PcsTeams = "TEAMS";

    public async Task Startlist(string raceName, int year, int raceId)
    {
        raceName ??= DB.Races.AsNoTracking().Single(r => r.RaceId == raceId).Name;
        year = year == 0 ? DB.Races.AsNoTracking().Single(r => r.RaceId == raceId).Year : year;
        raceId = raceId == 0 ? DB.Races.AsNoTracking().Single(r => r.Name == raceName && r.Year == year).RaceId : raceId;

        var html = await PcsClient.LoadAsync($"https://www.procyclingstats.com/race/{RaceString(raceName)}/{year}/startlist");
        var file = File.ReadAllText($"./api/Infrastructure/Scrape/{Filename(raceName)}.json");
        var json = JsonSerializer.Deserialize<PrijzenFile>(file);
        var query = StartlistQuery(raceId, html, json.Content);
        DB.Database.ExecuteSqlRaw(query);
    }

    public async Task StageResults(string raceName, int year, int stagenr)
        => await StageResults(DB.Stages.Include(s => s.Race).AsNoTracking().SingleOrDefault(s => s.Stagenr == stagenr && s.Race.Year == year && s.Race.Name == raceName));

    public async Task StageResults(Stage stage)
    {
        var stageNr = stage.Stagenr;
        var finishedOverride = false;
        if (stage.IsFinalStandings)
        {
            var mostRecentFinished = DB.Stages.OrderByDescending(s => s.Stagenr).FirstOrDefault(s => s.Finished && s.RaceId == stage.RaceId && s.Type != StageType.FinalStandings);
            if (mostRecentFinished is null) return;
            stageNr = mostRecentFinished.Stagenr;
            finishedOverride = stageNr != stage.Stagenr - 1;
            await CopyTeamsToStageSelections(stage);
        }
        var html = await PcsClient.LoadAsync($"https://www.procyclingstats.com/race/{RaceString(stage.Race.Name)}/{stage.Race.Year}/stage-{stageNr}");
        var classifications = html.QuerySelectorAll("a.selectResultTab").Select(x => x.InnerText);
        if (classifications.IsNullOrEmpty()) classifications = [PcsStage];
        var tables = html.QuerySelectorAll("#resultsCont .resTab").Select(x => x.QuerySelector("ul.ttt-results") ?? x.QuerySelector("table"));

        var (query, complete) = ResultsQuery(classifications.Zip(tables), stage, finishedOverride);
        if (query.Equals("")) return;
        ClearCache(query);
        await using (var transaction = await DB.Database.BeginTransactionAsync())
        {
            await DB.Database.ExecuteSqlRawAsync(query);
            await CalculateUserScores(stage);
            await transaction.CommitAsync();
        }

        var nextStage = DB.Stages.Include(s => s.Race).AsNoTracking().SingleOrDefault(x => x.Stagenr == stage.Stagenr + 1 && x.RaceId == stage.RaceId);
        if (nextStage?.Starttime < DateTime.Now) // Nodig als er meerdere etappes herberekend moeten worden, dan gaat dit door tot de huidige/laatste etappe
        {
            await StageResults(nextStage);
        }
        else if (stage.Stagenr is > 18 and < 22) // fictieve eindpunten berekenen voor de laatste paar etappes
        {
            var finalStandings = DB.Stages.Include(s => s.Race).AsNoTracking().SingleOrDefault(x => x.Type == StageType.FinalStandings && x.RaceId == stage.RaceId);
            await StageResults(finalStandings);
        }
    }

    public async Task<int> EtappesToevoegen(int raceId)
    {
        var (race, stageRows) = await GetRaceAndStageRows(raceId);

        var stageNr = 1;
        var starttime = new DateTime();
        var stages = DB.Stages.Where(s => s.RaceId == raceId).ToList();
        foreach (var row in stageRows)
        {
            starttime = await GetStartTime(row.QuerySelector("a").GetAttributeValue("href", ""));

            var stage = stages.SingleOrDefault(s => s.Stagenr == stageNr);
            if (stage is null)
            {
                stage = new Stage() { RaceId = raceId, Stagenr = stageNr };
                DB.Stages.Add(stage);
            }
            stage.Starttime = starttime;
            stage.Type = GetStageType(row.InnerText);
            stageNr++;
        }

        var finalStage = stages.SingleOrDefault(s => s.Stagenr == stageNr);
        if (finalStage is null)
        {
            finalStage = new Stage() { RaceId = raceId, Stagenr = stageNr };
            DB.Stages.Add(finalStage);
        }
        finalStage.Starttime = starttime;
        finalStage.Type = StageType.FinalStandings;

        return DB.SaveChanges();
    }

    private async Task<(Race race, IEnumerable<HtmlNode> stageRows)> GetRaceAndStageRows(int raceId)
    {
        var race = DB.Races.AsNoTracking().Single(r => r.RaceId == raceId);
        var html = await PcsClient.LoadAsync($"https://www.procyclingstats.com/race/{RaceString(race.Name)}/{race.Year}/");
        var rows = html.QuerySelector(".mt20 tbody").SelectNodes("tr");
        var stageRows = rows.Where(row => row.QuerySelector("a") != null && !row.InnerText.Contains("Restday"));
        return (race, stageRows);
    }

    public async Task DownloadStageProfiles(int raceId, string outputPath = "./wwwroot/profiles")
    {
        var (race, stageRows) = await GetRaceAndStageRows(raceId);
        using var httpClient = new HttpClient();

        var stageNr = 1;
        foreach (var row in stageRows)
        {
            var href = row.QuerySelector("a").GetAttributeValue("href", "");
            var stageFolder = Path.Combine(outputPath, RaceString(race.Name), race.Year.ToString(), $"stage-{stageNr}");
            Directory.CreateDirectory(stageFolder);

            var profilesHtml = await PcsClient.LoadAsync($"https://www.procyclingstats.com/{href}/info/profiles");
            var items = profilesHtml.QuerySelectorAll("ul.list li");

            var climbNr = 1;
            foreach (var item in items)
            {
                var label = item.QuerySelector(".fs14.bold")?.InnerText.Trim() ?? "";
                var imgSrc = item.QuerySelector("img")?.GetAttributeValue("src", "");
                if (string.IsNullOrEmpty(imgSrc)) continue;

                var ext = Path.GetExtension(imgSrc);
                string fileName = label switch
                {
                    "Profile" => $"profile{ext}",
                    "Finish profile" => $"finish-profile{ext}",
                    "Climb" => $"climb-{climbNr++}{ext}",
                    _ => null
                };

                if (fileName is null) continue;

                var imageUrl = $"https://www.procyclingstats.com/{imgSrc}";
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(Path.Combine(stageFolder, fileName), imageBytes);
            }

            stageNr++;
        }
    }

    private static StageType GetStageType(string innerText)
    {
        if (innerText.Contains("ITT")) return StageType.ITT;
        if (innerText.Contains("TTT")) return StageType.TTT;
        return StageType.REG;
    }

    private async static Task<DateTime> GetStartTime(string url)
    {
        var node = await PcsClient.LoadAsync($"https://www.procyclingstats.com/{url}");
        var raceInfo = node.QuerySelector(".w30 .keyvalueList").Children();
        var date = raceInfo.First(x => x.InnerText.Contains("Date")).Children().ToList()[1].InnerText;
        var time = raceInfo.First(x => x.InnerText.Contains("Start time")).Children().ToList()[1].InnerText.Split().First();
        if (!TimeSpan.TryParse(time, out _))
        {
            time = "12:00";
        }
        return Convert.ToDateTime(date + " " + time).AddHours(-2);
    }

    private async Task CopyTeamsToStageSelections(Stage stage)
    {
        var query = @$"INSERT INTO stage_selection_rider(stage_selection_id, rider_participation_id)
                    SELECT stage_selection_id, rider_participation_id FROM team_selection_rider
                    INNER JOIN stage_selection USING(account_participation_id)
                    INNER JOIN stage USING(stage_id)
                    WHERE race_id = {stage.RaceId} AND stagenr = {stage.Stagenr}
                    ON CONFLICT (stage_selection_id,rider_participation_id) DO NOTHING";
        await DB.Database.ExecuteSqlRawAsync(query);
    }

    private void ClearCache(string query)
    {
        if (MemoryCache.TryGetValue("ResultsUpdateQuery", out string cachedResult))
        {
            if (cachedResult != query)
            {
                if (MemoryCache is MemoryCache concreteMemoryCache)
                {
                    concreteMemoryCache.Clear();
                }
            }
        }
        MemoryCache.Set("ResultsUpdateQuery", query, TimeSpan.FromHours(24));
    }

    private async Task CalculateUserScores(Stage stage)
    {
        var stageSelections = DB.StageSelections.AsNoTracking().Where(ss => ss.Stage.StageId == stage.StageId).Include(ss => ss.AccountParticipation).ToList();

        foreach (var stageSelection in stageSelections)
        {
            var minusTeamPoints = "";
            var kopmanpunten = "stagescore/2";
            if (stageSelection.AccountParticipation.BudgetParticipation)
            {
                minusTeamPoints = stage.Type is StageType.TTT ? " - teamscore - (stagescore/2)" : " - teamscore";
                kopmanpunten = stage.Type is StageType.TTT ? "stagescore/4" : "stagescore/2";
            }

            var selectedRiders = $"(SELECT rider_participation_id FROM stage_selection_rider WHERE stage_selection_id = {stageSelection.StageSelectionId})";

            var stagescore = $"(SELECT SUM(totalscore {minusTeamPoints}) FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id IN {selectedRiders})";
            var kopmanscore = $"(SELECT {kopmanpunten} FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id = (SELECT kopman_id FROM stage_selection WHERE stage_selection_id = {stageSelection.StageSelectionId}))";
            var stageScoreTotal = $"COALESCE(({stagescore} + {kopmanscore}),0)";
            var query = $"UPDATE stage_selection SET stagescore = {stageScoreTotal} WHERE stage_selection_id = {stageSelection.StageSelectionId}; ";

            var prevTotal = DB.StageSelections.AsNoTracking().FirstOrDefault(ss => ss.AccountParticipationId == stageSelection.AccountParticipationId && ss.Stage.Stagenr == stage.Stagenr - 1)?.TotalScore ?? 0;
            var totalscore = $"{prevTotal} + {stageScoreTotal}";
            var updateTotals = $"UPDATE stage_selection SET totalscore = {totalscore} WHERE stage_selection.account_participation_id = {stageSelection.AccountParticipationId} AND stage_id IN (SELECT stage_id FROM stage WHERE stage.stagenr >= {stage.Stagenr} AND race_id = {stage.RaceId}); ";

            await DB.Database.ExecuteSqlRawAsync(query + updateTotals);
        }
    }

    private static string RaceString(string raceName)
        => raceName switch
        {
            "giro" => "giro-d-italia",
            "tour" => "tour-de-france",
            "vuelta" => "vuelta-a-espana",
            _ => throw new ArgumentOutOfRangeException()
        };

    private static string Filename(string raceName)
        => raceName switch
        {
            "giro" => "Giroprijzen",
            "tour" => "Tourprijzen",
            "vuelta" => "vueltaprijzen",
            _ => throw new ArgumentOutOfRangeException()
        };

    // internal DateTime? GetFinishTime()
    // {
    //     // TODO dynamic based on race
    //     var raceString = "Giro d'Italia";
    //     var html = new HtmlWeb().Load($"https://www.procyclingstats.com").DocumentNode;

    //     var raceRow = html.QuerySelectorAll("table.next-to-finish tr").FirstOrDefault(tr => tr.InnerText.Contains(raceString));
    //     return raceRow is null ? null : DateTime.Parse(raceRow.QuerySelectorAll("td").First().InnerText);
    // }
}