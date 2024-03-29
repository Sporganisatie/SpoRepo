using System.Text.Json;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    DatabaseContext DB;
    private IMemoryCache MemoryCache;

    public Scrape(DatabaseContext databaseContext, IMemoryCache memoryCache)
    {
        DB = databaseContext;
        MemoryCache = memoryCache;
    }

    public void Startlist(string raceName, int year)
    {
        var raceId = DB.Races.Single(r => r.Name == raceName && r.Year == year).RaceId;
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
        var stageNr = stage.IsFinalStandings ? stage.Stagenr - 1 : stage.Stagenr;
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(stage.Race.Name)}/{stage.Race.Year}/stage-{stageNr}").DocumentNode;
        var classifications = html.QuerySelectorAll(".restabs li a").Select(x => x.InnerText);
        var tables = html.QuerySelectorAll(".result-cont .subTabs")
                    .Where(x => x.GetAttributeValue("data-subtab", "") == "1")
                    .Select(x => x.QuerySelector("table"));
        var query = ResultsQuery(classifications.Zip(tables), stage);
        if (query.Equals("")) return;
        ClearCache(query);
        await DB.Database.ExecuteSqlRawAsync(query);

        CalculateUserScores(stage);
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
        // TODO if finalstandings use teamselections
        var stageSelections = DB.StageSelections.Where(ss => ss.Stage.StageId == stage.StageId).Include(ss => ss.AccountParticipation).ToList();

        foreach (var stageSelection in stageSelections)
        {
            var minusTeamPoints = "";
            if (stageSelection.AccountParticipation.BudgetParticipation)
            {
                minusTeamPoints = stage.Type is StageType.TTT ? " - teamscore - stagescore" : " - teamscore";
            }

            var selectedRiders = stage.IsFinalStandings
                ? $"(SELECT rider_participation_id FROM team_selection_rider WHERE account_participation_id = {stageSelection.AccountParticipationId})"
                : $"(SELECT rider_participation_id FROM stage_selection_rider WHERE stage_selection_id = {stageSelection.StageSelectionId})";

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

    private string RaceString(string raceName)
        => raceName switch
        {
            "giro" => "giro-d-italia",
            "tour" => "tour-de-france",
            "vuelta" => "vuelta-a-espana",
            _ => throw new ArgumentOutOfRangeException()
        };

    private string Filename(string raceName)
        => raceName switch
        {
            "giro" => "Giroprijzen",
            "tour" => "Tourprijzen",
            "vuelta" => "vueltaprijzen",
            _ => throw new ArgumentOutOfRangeException()
        };

    internal DateTime? GetFinishTime()
    {
        // TODO dynamic based on race
        var raceString = "Giro d'Italia";
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com").DocumentNode;

        var raceRow = html.QuerySelectorAll("table.next-to-finish tr").FirstOrDefault(tr => tr.InnerText.Contains(raceString));
        return raceRow is null ? null : DateTime.Parse(raceRow.QuerySelectorAll("td").First().InnerText);
    }
}