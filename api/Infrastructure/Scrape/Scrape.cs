using System.Text.Json;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape(DatabaseContext DB, IMemoryCache MemoryCache)
{
    public void Startlist(string raceName, int year, int raceId)
    {
        raceName ??= DB.Races.Single(r => r.RaceId == raceId).Name;
        year = year == 0 ? DB.Races.Single(r => r.RaceId == raceId).Year : year;
        raceId = raceId == 0 ? DB.Races.Single(r => r.Name == raceName && r.Year == year).RaceId : raceId;

        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(raceName)}/{year}/startlist").DocumentNode;
        var file = File.ReadAllText($"./api/Infrastructure/Scrape/{Filename(raceName)}.json");
        var json = JsonSerializer.Deserialize<PrijzenFile>(file);
        var query = StartlistQuery(raceId, html, json.Content);
        DB.Database.ExecuteSqlRaw(query);
    }

    public async Task StageResults(string raceName, int year, int stagenr)
        => await StageResults(DB.Stages.Include(s => s.Race).SingleOrDefault(s => s.Stagenr == stagenr && s.Race.Year == year && s.Race.Name == raceName));

    public async Task StageResults(Stage stage)
    {
        var stageNr = stage.Stagenr;
        if (stage.IsFinalStandings)
        {
            stageNr = stage.Stagenr - 1;
            await CopyTeamsToStageSelections(stage);
        }
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(stage.Race.Name)}/{stage.Race.Year}/stage-{stageNr}").DocumentNode;
        var classifications = html.QuerySelectorAll(".restabs li a").Select(x => x.InnerText);
        if (classifications.IsNullOrEmpty()) classifications = ["Stage"];
        var tables = html.QuerySelectorAll(".result-cont .subTabs")
                    .Where(x => x.GetAttributeValue("data-subtab", "") == "1")
                    .Select(x => x.QuerySelector("table"));
        var query = ResultsQuery(classifications.Zip(tables), stage);
        if (query.Equals("")) return;
        ClearCache(query);
        await DB.Database.ExecuteSqlRawAsync(query);

        CalculateUserScores(stage);
    }

    public async Task EtappesOphalen(int raceId)
    {
        var race = DB.Races.Single(r => r.RaceId == raceId);

        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(race.Name)}/{race.Year}/").DocumentNode;
        var table = html.QuerySelector(".mt20 tbody");
        var urls = table.SelectNodes("tr");

        var stageCount = 0;
        var starttime = new DateTime();
        var stages = new List<Stage>();
        foreach (var row in urls)
        {
            var url = row.QuerySelector("a");
            if (url == null) continue;
            stageCount++;
            starttime = GetStartTime(url.GetAttributeValue("href", ""));

            stages.Add(new Stage
            {
                RaceId = raceId,
                Type = GetStageType(url.InnerText),
                Starttime = starttime,
                Stagenr = stageCount
            });
        };
        stages.Add(new()
        {
            RaceId = raceId,
            Stagenr = stageCount + 1,
            Type = StageType.FinalStandings,
            Starttime = starttime
        });

        await BuildExecuteStageQuery(stages);
    }

    private async Task BuildExecuteStageQuery(List<Stage> stages)
    {
        var query = $"INSERT INTO stage(race_id, type, starttime, stagenr) \n VALUES";
        query += string.Join(",", stages.Select(x => $"({x.RaceId}, '{x.Type}', '{x.Starttime.Value:yyyy-MM-dd HH:mm} -02:00', {x.Stagenr})"));
        query += " ON CONFLICT (stagenr, race_id) DO UPDATE SET type = EXCLUDED.type, starttime = EXCLUDED.starttime";
        await DB.Database.ExecuteSqlRawAsync(query);
    }

    private static StageType GetStageType(string innerText)
    {
        if (innerText.Contains("ITT")) return StageType.ITT;
        if (innerText.Contains("TTT")) return StageType.TTT;
        return StageType.REG;
    }

    private static DateTime GetStartTime(string url)
    {
        var raceInfo = new HtmlWeb().Load($"https://www.procyclingstats.com/{url}")
                .DocumentNode.QuerySelector(".w30 .infolist").Children();
        var date = raceInfo.Where(x => x.InnerText.Contains("Date")).First().Children().ToList()[2].InnerText;
        var time = raceInfo.Where(x => x.InnerText.Contains("Start time")).First().Children().ToList()[2].InnerText.Split().First();
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

    private void CalculateUserScores(Stage stage)
    {
        var stageSelections = DB.StageSelections.Where(ss => ss.Stage.StageId == stage.StageId).Include(ss => ss.AccountParticipation).ToList();

        foreach (var stageSelection in stageSelections)
        {
            var minusTeamPoints = "";
            if (stageSelection.AccountParticipation.BudgetParticipation)
            {
                minusTeamPoints = stage.Type is StageType.TTT ? " - teamscore - stagescore" : " - teamscore";
            }

            var selectedRiders = $"(SELECT rider_participation_id FROM stage_selection_rider WHERE stage_selection_id = {stageSelection.StageSelectionId})";

            var stagescore = $"(SELECT SUM(totalscore {minusTeamPoints}) FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id IN {selectedRiders})";
            var kopmanscore = $"COALESCE((SELECT stagescore/2 FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id = (SELECT kopman_id FROM stage_selection WHERE stage_selection_id = {stageSelection.StageSelectionId})),0)";
            var stageScoreTotal = $"({stagescore} + {kopmanscore})";
            var query = $"UPDATE stage_selection SET stagescore = {stageScoreTotal} WHERE stage_selection_id = {stageSelection.StageSelectionId}; ";

            var prevTotal = DB.StageSelections.FirstOrDefault(ss => ss.AccountParticipationId == stageSelection.AccountParticipationId && ss.Stage.Stagenr == stage.Stagenr - 1)?.TotalScore ?? 0;
            var totalscore = $"{prevTotal} + COALESCE({stageScoreTotal},0)";
            var updateTotals = $"UPDATE stage_selection SET totalscore = {totalscore} WHERE stage_selection.account_participation_id = {stageSelection.AccountParticipationId} AND stage_id IN (SELECT stage_id FROM stage WHERE stage.stagenr >= {stage.Stagenr} AND race_id = {stage.RaceId}); ";

            DB.Database.ExecuteSqlRaw(query + updateTotals);
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