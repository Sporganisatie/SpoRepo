using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using SpoRE.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private string ResultsQuery(IEnumerable<(string Tab, HtmlNode Results)> classificationTables, Stage stage)
    {
        var riderResults = new Dictionary<string, RiderResult>();
        var teamWinners = new Dictionary<string, string>();
        foreach (var table in classificationTables)
        {
            ProcessResults(table.Tab, table.Results, ref riderResults, stage.Type, ref teamWinners);
        }
        UpdateTeamPoints(ref riderResults, teamWinners, classificationTables.Select(c => c.Tab), stage.Type);
        if (stage.Type is StageType.TTT) AddTTTResults(ref riderResults, classificationTables.Single(table => table.Tab == "").Results);
        UpdateDnfRiders(riderResults, stage);
        StageComplete(stage.StageId, riderResults);

        if (!riderResults.Any(r => !r.Value.Dnf)) return "";
        return BuildResultsQuery(riderResults.Values, stage);
    }

    private void StageComplete(int stageId, Dictionary<string, RiderResult> riderResults)
    {
        var stage = DB.Stages.Single(x => x.StageId == stageId);
        var dnfCount = riderResults.Count(r => r.Value.Dnf);
        var stageCount = riderResults.Count(r => r.Value.Stagepos != 0);
        var gcCount = riderResults.Count(r => r.Value.Gcpos != 0);
        var pointsCount = riderResults.Count(r => r.Value.Pointspos != 0);
        var komCount = riderResults.Count(r => r.Value.Kompos != 0);
        var yocCount = riderResults.Count(r => r.Value.Yocpos != 0);
        if (stage.Stagenr == 1)
        {
            var totalRiders = DB.RiderParticipations.Count(rp => rp.RaceId == stage.RaceId);
            var stageComplete = totalRiders == stageCount + dnfCount;
            var gcComplete = totalRiders == gcCount + dnfCount;
            var pointsComplete = pointsCount > 0;
            var komplete = komCount > 0; // Soms manual update nodig als bergpunten ontbreken in etappe 1
            var yocComplete = yocCount > 0;
            stage.Complete = stageComplete && gcComplete && pointsComplete && komplete && yocComplete;
        }
        else
        {
            var prevResult = DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stage.Stagenr - 1 && rp.Stage.RaceId == stage.RaceId);
            var stageComplete = prevResult.Count(p => p.StagePos != 0) == stageCount + dnfCount;
            var gcComplete = prevResult.Count(p => p.Gc.Position != 0) == gcCount + dnfCount;
            var pointsComplete = prevResult.Count(p => p.Points.Position != 0) <= pointsCount + dnfCount;
            var komplete = prevResult.Count(p => p.Kom.Position != 0) <= komCount + dnfCount;
            var yocComplete = prevResult.Count(p => p.Youth.Position != 0) <= yocCount + dnfCount;
            stage.Complete = stageComplete && gcComplete && pointsComplete && komplete && yocComplete;
        }
        stage.Complete = stage.Complete || stage.Type is StageType.TTT;
        stage.Finished = stageCount > 0;
        DB.SaveChanges();
    }

    private static string BuildResultsQuery(IEnumerable<RiderResult> riderResults, Stage stage)
    {
        return @$"DELETE FROM results_points WHERE stage_id = {stage.StageId}; INSERT INTO results_points(stage_id, rider_participation_id, 
                stagepos, stagescore, stageresult, gcpos, gcscore, gcresult, gcchange,
                pointspos, pointsscore, pointsresult, pointschange, kompos, komscore, komresult, komchange,
                yocpos, yocscore, yocresult, yocchange, teamscore, totalscore)
                VALUES" + string.Join(", ", riderResults.Where(x => !x.Dnf).Select(rider =>
                @$"(
                    {stage.StageId}, (SELECT rider_participation_id FROM rider_participation WHERE race_id = {stage.RaceId} AND rider_id = (SELECT rider_id FROM rider WHERE pcs_id = '{rider.PcsId}')),
                    {rider.Stagepos}, {rider.Stagescore}, '{rider.Stageresult}', {rider.Gcpos}, {rider.Gcscore}, '{rider.Gcresult}', '{rider.Gcchange}',
                    {rider.Pointspos}, {rider.Pointsscore}, '{rider.Pointsresult}', '{rider.Pointschange}', {rider.Kompos}, {rider.Komscore}, '{rider.Komresult}', '{rider.Komchange}',
                    {rider.Yocpos}, {rider.Yocscore}, '{rider.Yocresult}', '{rider.Yocchange}', {rider.Teamscore}, {rider.Totalscore}
                )"));
    }

    private static void UpdateTeamPoints(ref Dictionary<string, RiderResult> riderResults, Dictionary<string, string> teamWinners, IEnumerable<string> tabs, StageType type)
    {
        foreach (var rider in riderResults)
        {
            foreach (var tab in tabs)
            {
                if (tab == "Teams" || (tab is "Stage" or "" && type is StageType.ITT or StageType.TTT)) continue;
                rider.Value.Teamscore += TeamScore(rider.Value, teamWinners[tab], tab, type);
            }
            rider.Value.Totalscore += rider.Value.Teamscore;
        }
    }

    private void UpdateDnfRiders(Dictionary<string, RiderResult> riderResults, Stage stage)
    {
        var dnfRiders = riderResults.Values.Where(x => x.Dnf);
        if (!dnfRiders.Any()) return;
        var rpDnfQuery = $"UPDATE rider_participation SET dnf = TRUE WHERE race_id = {stage.RaceId} AND rider_id IN("
                    + string.Join(",", riderResults.Values.Where(x => x.Dnf).Select(rider => $"(SELECT rider_id FROM rider WHERE pcs_id = '{rider.PcsId}')"))
                    + "); ";

        var dnfRpIds = $"(SELECT rider_participation_id FROM rider_participation WHERE race_id = {stage.RaceId} AND dnf)";
        var stageNrForDeletions = stage.Starttime > DateTime.Now ? stage.Stagenr : stage.Stagenr + 1;
        var futureSelections = $"(SELECT stage_selection_id FROM stage_selection INNER JOIN stage USING(stage_id) WHERE race_id = {stage.RaceId} AND stagenr >= {stageNrForDeletions})";
        var removeFromStageSelectionsQuery = @$"DELETE FROM stage_selection_rider 
                WHERE rider_participation_id IN {dnfRpIds} AND stage_selection_id IN {futureSelections}; ";

        DB.Database.ExecuteSqlRaw(rpDnfQuery + removeFromStageSelectionsQuery);
    }

    private static void ProcessResults(string tab, HtmlNode htmlResults, ref Dictionary<string, RiderResult> riderResults, StageType type, ref Dictionary<string, string> teamWinners)
    {
        if (tab == "Teams" || (tab == "" && type is StageType.TTT)) return;
        var pcsRows = ResultsDict(htmlResults);
        teamWinners[tab] = pcsRows.FirstOrDefault()?.Team;
        foreach (var pcsRow in pcsRows)
        {
            riderResults.TryAdd(pcsRow.PcsId, new(pcsRow.PcsId, pcsRow.Team));
            riderResults[pcsRow.PcsId] = AddResults(riderResults[pcsRow.PcsId], pcsRow, tab, type);
        }
        foreach (var (key, value) in riderResults)
        {
            riderResults[key] = value with { Totalscore = value.Stagescore + value.Gcscore + value.Pointsscore + value.Komscore + value.Yocscore };
        }
        return;
    }

    private static IEnumerable<PcsRow> ResultsDict(HtmlNode htmlResults)
    {
        var columns = htmlResults.QuerySelectorAll("th").Select(x => x.InnerText);
        var rows = htmlResults.QuerySelectorAll("tbody tr");
        var results = rows.Select(row => BuildPcsRider(columns, row));
        return rows.Select(row => BuildPcsRider(columns, row)).Where(x => x.PcsId != "skip-rider");
    }

    private static PcsRow BuildPcsRider(IEnumerable<string> columns, HtmlNode row)
    {
        var fields = columns.Zip(row.QuerySelectorAll("td"), (col, val) => new { col, val }).ToDictionary(x => x.col, x => x.val);
        if (!fields.ContainsKey("Rider")) return new() { PcsId = "skip-rider" };
        var pcsId = fields["Rider"].QuerySelector("a").GetAttributeValue("href", "")[6..];
        if (!int.TryParse(fields["Rnk"].InnerText, out int rank)) return new() { Dnf = true, PcsId = pcsId };

        return new()
        {
            Rank = rank,
            RankChange = GetRankChange(fields),
            PcsId = pcsId,
            Time = GetTime(fields),
            Team = GetString(fields, "Team"),
            Points = GetString(fields, "Points"),
        };
    }

    private static string GetRankChange(Dictionary<string, HtmlNode> fields)
    {
        var change = GetString(fields, "&#x25BC;&#x25B2;");
        var prev = GetString(fields, "Prev");
        return prev == "" ? "*" : change.Replace("&#x25BC;", "▼").Replace("&#x25B2;", "▲");
    }

    private static string GetTime(Dictionary<string, HtmlNode> fields)
    {
        if (!fields.TryGetValue("Time", out HtmlNode value)) return "";
        return value.SelectSingleNode(".//text()[normalize-space()]")?.InnerHtml ?? "";
    }

    private static string GetString(Dictionary<string, HtmlNode> fields, string col)
        => fields.TryGetValue(col, out HtmlNode value) ? value.InnerText : "";

    private static RiderResult AddResults(RiderResult riderResult, PcsRow pcsRow, string tab, StageType type)
        => tab switch
        {
            "" or "Stage" => riderResult with
            {
                Stagepos = pcsRow.Rank,
                Stageresult = pcsRow.Time,
                Stagescore = Score(pcsRow.Rank, tab, type),
                Dnf = pcsRow.Dnf
            },
            "GC" => riderResult with
            {
                Gcpos = pcsRow.Rank,
                Gcresult = pcsRow.Time,
                Gcscore = Score(pcsRow.Rank, tab, type),
                Gcchange = pcsRow.RankChange
            },
            "Points" => riderResult with
            {
                Pointspos = pcsRow.Rank,
                Pointsresult = pcsRow.Points,
                Pointsscore = Score(pcsRow.Rank, tab, type),
                Pointschange = pcsRow.RankChange
            },
            "KOM" => riderResult with
            {
                Kompos = pcsRow.Rank,
                Komresult = pcsRow.Points,
                Komscore = Score(pcsRow.Rank, tab, type),
                Komchange = pcsRow.RankChange
            },
            "Youth" => riderResult with
            {
                Yocpos = pcsRow.Rank,
                Yocresult = pcsRow.Time,
                Yocscore = Score(pcsRow.Rank, tab, type),
                Yocchange = pcsRow.RankChange
            },
            _ => riderResult
        };

    private static void AddTTTResults(ref Dictionary<string, RiderResult> riderResults, HtmlNode Results)
    {
        var teamOrder = Results.QuerySelectorAll(".team").Select(x => x.QuerySelectorAll("td").ElementAt(1).InnerText.Trim()).ToList();
        // var times = new List<string>(); TODO get
        foreach (var (key, value) in riderResults)
        {
            var position = teamOrder.IndexOf(value.Team) + 1;
            riderResults[key] = value with
            {
                Stagepos = position,
                Stageresult = "Todo finish time",
                Stagescore = Score(position, "Stage", StageType.TTT),
                Totalscore = riderResults[key].Totalscore + Score(position, "Stage", StageType.TTT)
            };
        }
    }
}

internal record PcsRow
{
    public bool Dnf { get; set; }
    public int Rank { get; set; }
    public string RankChange { get; set; }
    public string PcsId { get; set; }
    public string Time { get; set; }
    public string Team { get; set; }
    public string Points { get; set; }
}

internal record RiderResult(string PcsId, string Team)
{
    public bool Dnf { get; set; }
    public int Stagepos { get; set; }
    public int Gcpos { get; set; }
    public int Pointspos { get; set; }
    public int Kompos { get; set; }
    public int Yocpos { get; set; }
    public int Stagescore { get; set; }
    public int Gcscore { get; set; }
    public int Pointsscore { get; set; }
    public int Komscore { get; set; }
    public int Yocscore { get; set; }
    public int Teamscore { get; set; }
    public int Totalscore { get; set; }
    public string Stageresult { get; set; }
    public string Gcresult { get; set; }
    public string Pointsresult { get; set; }
    public string Komresult { get; set; }
    public string Yocresult { get; set; }
    public string Gcchange { get; set; }
    public string Pointschange { get; set; }
    public string Komchange { get; set; }
    public string Yocchange { get; set; }
}