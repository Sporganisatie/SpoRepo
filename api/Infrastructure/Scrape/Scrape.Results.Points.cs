namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private int Score(int rank, string tab)
        => tab switch
        {
            "" => rank > 20 ? 0 : new int[] { 0, 50, 44, 40, 36, 32, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 }[rank],
            "GC" => rank > 5 ? 0 : new int[] { 0, 10, 8, 6, 4, 2 }[rank],
            "Points" => rank > 5 ? 0 : new int[] { 0, 8, 6, 4, 2, 1 }[rank],
            "KOM" => rank > 5 ? 0 : new int[] { 0, 6, 4, 3, 2, 1 }[rank],
            "Youth" => rank > 3 ? 0 : new int[] { 0, 5, 3, 1 }[rank],
            _ => 0
        };

    // private int TeamScore(PcsRow pcsRow, string classification, string teamWinner, string stageType)
    // {
    //     if (pcsRow.Rank == 1 || pcsRow.Team != teamWinner) return 0;
    //     var teampointsDict = stageType switch
    //     {
    //         "REG" => new Dictionary<string, int>() { { "", 10 }, { "GC", 8 }, { "Points", 6 }, { "KOM", 3 }, { "Youth", 2 } },
    //         "ITT" => new Dictionary<string, int>() { { "", 0 }, { "GC", 8 }, { "Points", 6 }, { "KOM", 3 }, { "Youth", 2 } },
    //         "TTT" => new Dictionary<string, int>() { { "", 0 }, { "GC", 8 }, { "Points", 6 }, { "KOM", 3 }, { "Youth", 2 } },
    //         "FinalStandings" => new Dictionary<string, int>() { { "", 0 }, { "GC", 24 }, { "Points", 18 }, { "KOM", 9 }, { "Youth", 6 } },
    //         _ => throw new ArgumentOutOfRangeException()
    //     };
    //     return teampointsDict.TryGetValue(classification, out var teampoints) ? teampoints : 0;
    // }
}