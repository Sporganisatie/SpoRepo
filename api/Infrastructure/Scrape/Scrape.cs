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

    public void StageResults(string raceName, int year, int stagenr)
    {
        var stage = DB.Stages.SingleOrDefault(s => s.Stagenr == stagenr && s.Race.Year == year && s.Race.Name == raceName);
        HtmlWeb web = new HtmlWeb();
        var html = web.Load($"https://www.procyclingstats.com/race/{RaceString(raceName)}/{year}/stage-{stagenr}").DocumentNode;
        var classifications = html.QuerySelectorAll(".restabs li a").Select(x => x.InnerText);
        var tables = html.QuerySelectorAll(".result-cont .subTabs")
                    .Where(x => x.GetAttributeValue("data-subtab", "") == "1")
                    .Select(x => x.QuerySelector("table"));
        var query = ResultsQuery(classifications.Zip(tables), stage);
        DB.Database.ExecuteSqlRaw(query);

        CalculateUserScores(stage.StageId);
    }

    private void CalculateUserScores(int stageId)
    {
        var stageSelections = DB.StageSelections.Where(ss => ss.Stage.StageId == stageId).Include(ss => ss.AccountParticipation).ToList();

        foreach (var stageSelection in stageSelections)
        {
            var minusTeamPoints = stageSelection.AccountParticipation.Budgetparticipation ?? false ? " - teamscore" : "";
            var selectedRiders = $"(SELECT rider_participation_id FROM stage_selection_rider WHERE stage_selection_id = {stageSelection.StageSelectionId})";
            var stagescore = $"(SELECT SUM(totalscore {minusTeamPoints}) FROM results_points WHERE stage_id = {stageId} AND rider_participation_id IN {selectedRiders})";
            var kopmanscore = $"(SELECT stagescore/2 FROM results_points WHERE stage_id = {stageId} AND rider_participation_id = (SELECT kopman_id FROM stage_selection WHERE stage_selection_id = {stageSelection.StageSelectionId}))";
            var query = $"UPDATE stage_selection SET stagescore = ({stagescore} + {kopmanscore}) WHERE stage_selection_id = {stageSelection.StageSelectionId}";
            DB.Database.ExecuteSqlRaw(query);
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
}