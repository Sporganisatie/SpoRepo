using System.Text.Json;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    DatabaseContext DB;
    public Scrape(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public void Startlist(string raceName, int year)
    {
        var raceId = DB.Races.Single(r => r.Name == raceName && r.Year == year).RaceId;
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(raceName)}/{year}/startlist").DocumentNode;
        var file = File.ReadAllText($"./api/Infrastructure/Scrape/{Filename(raceName)}.txt");
        var json = JsonSerializer.Deserialize<PrijzenFile>(file);
        var riderQualities = json.Content; // TODO remove hardcoded
        var query = StartlistQuery(raceId, html, riderQualities);
        DB.Database.ExecuteSqlRaw(query);
    }

    public async Task StageResults(string raceName, int year, int stagenr)
        => await StageResults(DB.Stages.Include(s => s.Race).SingleOrDefault(s => s.Stagenr == stagenr && s.Race.Year == year && s.Race.Name == raceName));

    public async Task StageResults(Stage stage)
    {
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(stage.Race.Name)}/{stage.Race.Year}/stage-{stage.Stagenr}").DocumentNode;
        var classifications = html.QuerySelectorAll(".restabs li a").Select(x => x.InnerText);
        var tables = html.QuerySelectorAll(".result-cont .subTabs")
                    .Where(x => x.GetAttributeValue("data-subtab", "") == "1")
                    .Select(x => x.QuerySelector("table"));
        var query = ResultsQuery(classifications.Zip(tables), stage);
        if (query.Equals("")) return;
        await DB.Database.ExecuteSqlRawAsync(query);

        CalculateUserScores(stage);
    }

    private void CalculateUserScores(Stage stage)
    {
        var stageSelections = DB.StageSelections.Where(ss => ss.Stage.StageId == stage.StageId).Include(ss => ss.AccountParticipation).ToList();

        foreach (var stageSelection in stageSelections)
        {
            var minusTeamPoints = stageSelection.AccountParticipation.BudgetParticipation ? " - teamscore" : "";
            var selectedRiders = $"(SELECT rider_participation_id FROM stage_selection_rider WHERE stage_selection_id = {stageSelection.StageSelectionId})";
            var stagescore = $"(SELECT SUM(totalscore {minusTeamPoints}) FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id IN {selectedRiders})";
            var kopmanscore = $"COALESCE((SELECT stagescore/2 FROM results_points WHERE stage_id = {stage.StageId} AND rider_participation_id = (SELECT kopman_id FROM stage_selection WHERE stage_selection_id = {stageSelection.StageSelectionId})),0)";
            var stageScoreTotal = $"({stagescore} + {kopmanscore})";
            var query = $"UPDATE stage_selection SET stagescore = {stageScoreTotal} WHERE stage_selection_id = {stageSelection.StageSelectionId}; ";

            var prevTotal = stage.Stagenr != 1 ? DB.StageSelections.Single(ss => ss.AccountParticipationId == stageSelection.AccountParticipationId && ss.Stage.Stagenr == stage.Stagenr - 1).TotalScore : 0;
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
            // "tour" => "Filename",
            // "vuelta" => "Filename",
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