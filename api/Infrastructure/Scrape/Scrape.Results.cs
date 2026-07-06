using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using SpoRE.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private (string query, bool complete) ResultsQuery(IEnumerable<(string Tab, HtmlNode Results)> classificationTables, Stage stage, bool finishedOverride)
    {
        var riderResults = new Dictionary<int, RiderResult>();
        var teamWinners = new Dictionary<string, string>();
        foreach (var table in classificationTables)
        {
            ProcessResults(table.Tab, table.Results, ref riderResults, stage.Type, ref teamWinners);
        }
        UpdateTeamPoints(ref riderResults, teamWinners, classificationTables.Select(c => c.Tab), stage.Type);
        if (stage.Type is StageType.TTT)
        {
            AddTTTResults(ref riderResults, classificationTables.Single(table => table.Tab == PcsStage).Results);
        }
        UpdateDnfRiders(riderResults, stage);
        var complete = StageComplete(stage.StageId, riderResults, finishedOverride);

        if (!riderResults.Any(r => !r.Value.Dnf)) return ("", false);
        return (BuildResultsQuery(riderResults.Values, stage), complete);
    }

    private bool StageComplete(int stageId, Dictionary<int, RiderResult> riderResults, bool finishedOverride)
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
            var stageComplete = (totalRiders == stageCount + dnfCount) || stage.Type is StageType.TTT;
            var gcComplete = totalRiders == gcCount + dnfCount;
            var pointsComplete = pointsCount > 0;
            var komplete = komCount > 0; // Soms manual update nodig als bergpunten ontbreken in etappe 1
            var yocComplete = yocCount > 0;
            stage.Complete = stageComplete && gcComplete && pointsComplete && komplete && yocComplete;
        }
        else
        {
            var prevResult = DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stage.Stagenr - 1 && rp.Stage.RaceId == stage.RaceId);
            var stageComplete = (prevResult.Count(p => p.StagePos != 0) == stageCount + dnfCount) || stage.Type is StageType.TTT;
            var gcComplete = prevResult.Count(p => p.Gc.Position != 0) == gcCount + dnfCount;
            var pointsComplete = prevResult.Count(p => p.Points.Position != 0) <= pointsCount + dnfCount;
            var komplete = prevResult.Count(p => p.Kom.Position != 0) <= komCount + dnfCount;
            var yocComplete = prevResult.Count(p => p.Youth.Position != 0) <= yocCount + dnfCount;
            stage.Complete = stageComplete && gcComplete && pointsComplete && komplete && yocComplete;
        }
        stage.Complete = stage.Complete || stage.StageId is 965 or 966 or 975;
        stage.Finished = stageCount > 0 && !finishedOverride;
        DB.SaveChanges();

        return stage.Complete;
    }

    private static string BuildResultsQuery(IEnumerable<RiderResult> riderResults, Stage stage)
    {
        return @$"DELETE FROM results_points WHERE stage_id = {stage.StageId};
                INSERT INTO results_points(stage_id, rider_participation_id, 
                stagepos, stagescore, stageresult, gcpos, gcscore, gcresult, gcchange,
                pointspos, pointsscore, pointsresult, pointschange, kompos, komscore, komresult, komchange,
                yocpos, yocscore, yocresult, yocchange, teamscore, totalscore)
                VALUES" + string.Join(", ", riderResults.Where(x => !x.Dnf).Select(rider =>
                @$"(
                    {stage.StageId}, (SELECT rider_participation_id FROM rider_participation WHERE race_id = {stage.RaceId} AND startnummer = {rider.Startnummer}),
                    {rider.Stagepos}, {rider.Stagescore}, '{rider.Stageresult}', {rider.Gcpos}, {rider.Gcscore}, '{rider.Gcresult}', '{rider.Gcchange}',
                    {rider.Pointspos}, {rider.Pointsscore}, '{rider.Pointsresult}', '{rider.Pointschange}', {rider.Kompos}, {rider.Komscore}, '{rider.Komresult}', '{rider.Komchange}',
                    {rider.Yocpos}, {rider.Yocscore}, '{rider.Yocresult}', '{rider.Yocchange}', {rider.Teamscore}, {rider.Totalscore}
                )"));
    }

    private static void UpdateTeamPoints(ref Dictionary<int, RiderResult> riderResults, Dictionary<string, string> teamWinners, IEnumerable<string> tabs, StageType type)
    {
        foreach (var rider in riderResults)
        {
            foreach (var tab in tabs)
            {
                if (tab == PcsTeams || (tab is PcsStage or "" && type is StageType.ITT or StageType.TTT)) continue;
                rider.Value.Teamscore += TeamScore(rider.Value, teamWinners[tab], tab, type);
            }
            rider.Value.Totalscore += rider.Value.Teamscore;
        }
    }

    private void UpdateDnfRiders(Dictionary<int, RiderResult> riderResults, Stage stage)
    {
        var dnfRiders = riderResults.Values.Where(x => x.Dnf).ToList();
        if (!dnfRiders.Any()) return;
        var rpDnfQuery = $"UPDATE rider_participation SET dnf = TRUE WHERE race_id = {stage.RaceId} AND startnummer IN("
                    + string.Join(",", dnfRiders.Select(rider => rider.Startnummer))
                    + "); ";

        var dnfRpIds = $"(SELECT rider_participation_id FROM rider_participation WHERE race_id = {stage.RaceId} AND dnf)";
        var futureSelections = $"(SELECT stage_selection_id FROM stage_selection INNER JOIN stage USING(stage_id) WHERE race_id = {stage.RaceId} AND stage.starttime > NOW())";
        var removeFromStageSelectionsQuery = @$"DELETE FROM stage_selection_rider 
                WHERE rider_participation_id IN {dnfRpIds} AND stage_selection_id IN {futureSelections}; ";

        DB.Database.ExecuteSqlRaw(rpDnfQuery + removeFromStageSelectionsQuery);
    }

    private static void ProcessResults(string tab, HtmlNode htmlResults, ref Dictionary<int, RiderResult> riderResults, StageType type, ref Dictionary<string, string> teamWinners)
    {
        if (tab == PcsTeams || (tab == "" && type is StageType.TTT)) return;
        var pcsRows = ResultsDict(htmlResults);
        teamWinners[tab] = pcsRows.FirstOrDefault(x => x.Rank != 0)?.Team;
        foreach (var pcsRow in pcsRows)
        {
            riderResults.TryAdd(pcsRow.Startnummer, new(pcsRow.Startnummer, pcsRow.Team));
            riderResults[pcsRow.Startnummer] = AddResults(riderResults[pcsRow.Startnummer], pcsRow, tab, type);
        }
        foreach (var (key, value) in riderResults)
        {
            riderResults[key] = value with { Totalscore = value.Stagescore + value.Gcscore + value.Pointsscore + value.Komscore + value.Yocscore };
        }
        return;
    }

    private static IEnumerable<PcsRow> ResultsDict(HtmlNode htmlResults)
    {
        var columns = htmlResults.QuerySelectorAll("th").Select(x => x.InnerText).ToList();
        int? colToIgnore = null;

        if (columns.Count(x => x == "Pnt") > 1) // Dubbele Pnt kolommen bij eindstanden punten en berg klassementen
        {
            var pntIndices = columns
                .Select((col, idx) => new { col, idx })
                .Where(x => x.col == "Pnt")
                .Select(x => x.idx)
                .ToList();
            colToIgnore = pntIndices.First(); // De eerste Pnt kolom zijn de pcs punten
            columns.RemoveAt(colToIgnore.Value);
        }

        var rows = htmlResults.QuerySelectorAll("tbody tr");
        return rows.Select(row => BuildPcsRider(columns, row, colToIgnore)).Where(x => x.PcsId != "skip-rider" && x.Startnummer > 0);
    }

    private static PcsRow BuildPcsRider(List<string> columns, HtmlNode row, int? colToIgnore)
    {
        var values = row.QuerySelectorAll("td").ToList();
        if (colToIgnore.HasValue)
        {
            values.RemoveAt(colToIgnore.Value);
        }

        var fields = columns.Zip(values, (col, val) => new { col, val }).ToDictionary(x => x.col, x => x.val);
        if (!fields.TryGetValue("Rider", out HtmlNode value)) return new() { PcsId = "skip-rider" };
        var bibText = row.QuerySelector("td.bibs")?.InnerText?.Trim();
        var startnummer = int.TryParse(bibText, out var parsedBib) ? parsedBib : 0;
        var pcsId = value.QuerySelector("a").GetAttributeValue("href", "")[6..];

        var rnkText = GetString(fields, "Rnk");
        int rank;
        if (rnkText is "DF" or "NR")
        {
            rank = 0;
        }
        else if (!int.TryParse(rnkText, out rank))
        {
            return new() { Dnf = true, Startnummer = startnummer, PcsId = pcsId };
        }

        return new()
        {
            Rank = rank,
            RankChange = GetRankChange(fields),
            Startnummer = startnummer,
            PcsId = pcsId,
            Time = GetTime(fields),
            Team = GetString(fields, "Team"),
            Points = GetString(fields, "Pnt"),
        };
    }

    private static string GetRankChange(Dictionary<string, HtmlNode> fields)
    {
        var change = GetFirstNonEmpty(fields, "▼▲", "&#x25BC;&#x25B2;", "Delta", "delta");
        var prev = GetString(fields, "Prev");
        var normalizedChange = change
            .Replace("&#x25BC;", "▼")
            .Replace("&#x25B2;", "▲")
            .Trim();

        return prev == "" ? "*" : normalizedChange;
    }

    private static string GetTime(Dictionary<string, HtmlNode> fields)
    {
        if (!fields.TryGetValue("Time", out HtmlNode value)) return "";
        return value.SelectSingleNode(".//text()[normalize-space()]")?.InnerHtml ?? "";
    }

    private static string GetString(Dictionary<string, HtmlNode> fields, string col)
        => fields.TryGetValue(col, out HtmlNode value) ? value.InnerText : "";

    private static string GetFirstNonEmpty(Dictionary<string, HtmlNode> fields, params string[] columns)
    {
        foreach (var col in columns)
        {
            var value = GetString(fields, col).Trim();
            if (value != "") return value;
        }

        return "";
    }

    private static RiderResult AddResults(RiderResult riderResult, PcsRow pcsRow, string tab, StageType type)
        => tab switch
        {
            "" or PcsStage => riderResult with
            {
                Stagepos = pcsRow.Rank,
                Stageresult = pcsRow.Time,
                Stagescore = Score(pcsRow.Rank, tab, type),
                Dnf = pcsRow.Dnf
            },
            PcsGc => riderResult with
            {
                Gcpos = pcsRow.Rank,
                Gcresult = pcsRow.Time,
                Gcscore = Score(pcsRow.Rank, tab, type),
                Gcchange = pcsRow.RankChange
            },
            PcsPoints => riderResult with
            {
                Pointspos = pcsRow.Rank,
                Pointsresult = pcsRow.Points,
                Pointsscore = Score(pcsRow.Rank, tab, type),
                Pointschange = pcsRow.RankChange
            },
            PcsKom => riderResult with
            {
                Kompos = pcsRow.Rank,
                Komresult = pcsRow.Points,
                Komscore = Score(pcsRow.Rank, tab, type),
                Komchange = pcsRow.RankChange
            },
            PcsYouth => riderResult with
            {
                Yocpos = pcsRow.Rank,
                Yocresult = pcsRow.Time,
                Yocscore = Score(pcsRow.Rank, tab, type),
                Yocchange = pcsRow.RankChange
            },
            _ => riderResult
        };

    private static void AddTTTResults(ref Dictionary<int, RiderResult> riderResults, HtmlNode Results)
    {
        var teamOrder = Results.ChildNodes.Skip(1).Select(li => li.QuerySelectorAll("a").First().InnerText.Trim()).ToList();
        foreach (var (key, value) in riderResults)
        {
            var position = teamOrder.IndexOf(value.Team) + 1;
            riderResults[key] = value with
            {
                Stagepos = position,
                Stageresult = "Todo finish time",
                Stagescore = Score(position, PcsStage, StageType.TTT),
                Totalscore = riderResults[key].Totalscore + Score(position, PcsStage, StageType.TTT)
            };
        }
    }
}

internal record PcsRow
{
    public bool Dnf { get; set; }
    public int Rank { get; set; }
    public string RankChange { get; set; }
    public int Startnummer { get; set; }
    public string PcsId { get; set; }
    public string Time { get; set; }
    public string Team { get; set; }
    public string Points { get; set; }
}

internal record RiderResult(int Startnummer, string Team)
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