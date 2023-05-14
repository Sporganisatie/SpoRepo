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
        UpdateDnfRiders(riderResults, stage);
        StageComplete(stage, riderResults);

        if (DB.Stages.Count(s => s.RaceId == stage.RaceId) == stage.Stagenr + 1)
        {
            // eindklassement
        }
        if (!riderResults.Any()) return "";
        return BuildResultsQuery(riderResults.Values, stage);
    }

    private void StageComplete(Stage stage, Dictionary<string, RiderResult> riderResults)
    {
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
            var stageComplete = prevResult.Count(p => p.Stagepos != 0) == stageCount + dnfCount;
            var gcComplete = prevResult.Count(p => p.Gcpos != 0) == gcCount + dnfCount;
            var pointsComplete = prevResult.Count(p => p.Pointspos != 0) <= pointsCount + dnfCount;
            var komplete = prevResult.Count(p => p.Kompos != 0) <= komCount + dnfCount;
            var yocComplete = prevResult.Count(p => p.Yocpos != 0) <= yocCount + dnfCount;
            stage.Complete = stageComplete && gcComplete && pointsComplete && komplete && yocComplete;
        }

        stage.Finished = stageCount > 0;
        DB.SaveChanges();
    }

    private string BuildResultsQuery(IEnumerable<RiderResult> riderResults, Stage stage)
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

    private void UpdateTeamPoints(ref Dictionary<string, RiderResult> riderResults, Dictionary<string, string> teamWinners, IEnumerable<string> tabs, string type)
    {
        foreach (var rider in riderResults)
        {
            foreach (var tab in tabs)
            {
                if (tab == "Teams") continue;
                rider.Value.Teamscore += TeamScore(rider.Value, teamWinners[tab], tab, type);
            }
            rider.Value.Totalscore += rider.Value.Teamscore;
        }
    }

    private void UpdateDnfRiders(Dictionary<string, RiderResult> riderResults, Stage stage)
    {
        var dnfRiders = riderResults.Values.Where(x => x.Dnf);
        if (!dnfRiders.Any()) return;
        var query = $"UPDATE rider_participation SET dnf = TRUE WHERE race_id = {stage.RaceId} AND rider_id IN("
                    + string.Join(",", riderResults.Values.Where(x => x.Dnf).Select(rider => $"(SELECT rider_id FROM rider WHERE pcs_id = '{rider.PcsId}')"))
                    + "); ";
        DB.Database.ExecuteSqlRaw(query);
    }

    private void ProcessResults(string tab, HtmlNode htmlResults, ref Dictionary<string, RiderResult> riderResults, string type, ref Dictionary<string, string> teamWinners)
    {
        if (tab == "Teams") return;
        var pcsRows = ResultsDict(htmlResults);
        teamWinners[tab] = pcsRows.FirstOrDefault()?.Team;
        foreach (var pcsRow in pcsRows)
        {
            riderResults.TryAdd(pcsRow.PcsId, new(pcsRow.PcsId, pcsRow.Team));
            riderResults[pcsRow.PcsId] = AddResults(riderResults[pcsRow.PcsId], pcsRow, tab);
        }
        foreach (var (key, value) in riderResults)
        {
            riderResults[key] = value with { Totalscore = value.Stagescore + value.Gcscore + value.Pointsscore + value.Komscore + value.Yocscore };
        }
        return;
    }

    private IEnumerable<PcsRow> ResultsDict(HtmlNode htmlResults)
    {
        var columns = htmlResults.QuerySelectorAll("th").Select(x => x.InnerText);
        var rows = htmlResults.QuerySelectorAll("tbody tr");
        var results = rows.Select(row => BuildPcsRider(columns, row));
        return rows.Select(row => BuildPcsRider(columns, row));
    }

    private PcsRow BuildPcsRider(IEnumerable<string> columns, HtmlNode row)
    {
        var fields = columns.Zip(row.QuerySelectorAll("td"), (col, val) => new { col, val }).ToDictionary(x => x.col, x => x.val);
        var rank = 0;
        var pcsId = fields["Rider"].QuerySelector("a").GetAttributeValue("href", "").Substring(6);
        if (!int.TryParse(fields["Rnk"].InnerText, out rank)) return new() { Dnf = true, PcsId = pcsId };

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

    private string GetRankChange(Dictionary<string, HtmlNode> fields)
    {
        var change = GetString(fields, "&#x25BC;&#x25B2;");
        var prev = GetString(fields, "Prev");
        return prev == "" ? "*" : change.Replace("&#x25BC;", "▼").Replace("&#x25B2;", "▲");
    }

    private string GetTime(Dictionary<string, HtmlNode> fields)
    {
        if (!fields.ContainsKey("Time")) return "";
        var timeElement = fields["Time"].SelectSingleNode(".//text()[normalize-space()]");
        return timeElement is not null ? timeElement.InnerHtml : "";
    }

    private string GetString(Dictionary<string, HtmlNode> fields, string col)
        => fields.ContainsKey(col) ? fields[col].InnerText : "";

    private RiderResult AddResults(RiderResult riderResult, PcsRow pcsRow, string tab)
        => tab switch
        {
            "" or "Stage" => riderResult with
            {
                Stagepos = pcsRow.Rank,
                Stageresult = pcsRow.Time,
                Stagescore = Score(pcsRow.Rank, tab),
                Dnf = pcsRow.Dnf
            },
            "GC" => riderResult with
            {
                Gcpos = pcsRow.Rank,
                Gcresult = pcsRow.Time,
                Gcscore = Score(pcsRow.Rank, tab),
                Gcchange = pcsRow.RankChange
            },
            "Points" => riderResult with
            {
                Pointspos = pcsRow.Rank,
                Pointsresult = pcsRow.Points,
                Pointsscore = Score(pcsRow.Rank, tab),
                Pointschange = pcsRow.RankChange
            },
            "KOM" => riderResult with
            {
                Kompos = pcsRow.Rank,
                Komresult = pcsRow.Points,
                Komscore = Score(pcsRow.Rank, tab),
                Komchange = pcsRow.RankChange
            },
            "Youth" => riderResult with
            {
                Yocpos = pcsRow.Rank,
                Yocresult = pcsRow.Time,
                Yocscore = Score(pcsRow.Rank, tab),
                Yocchange = pcsRow.RankChange
            },
            _ => riderResult
        };
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