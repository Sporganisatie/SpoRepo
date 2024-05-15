using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private static int Score(int rank, string tab, StageType type)
        => type is StageType.FinalStandings ? EindScore(rank, tab) : StageScore(rank, tab, type);

    private static int StageScore(int rank, string tab, StageType type)
        => tab switch
        {
            "" or "Stage" => type is StageType.TTT
                ? rank > 8 ? 0 : new int[] { 0, 40, 32, 28, 24, 20, 16, 12, 8 }[rank]
                : rank > 20 ? 0 : new int[] { 0, 50, 44, 40, 36, 32, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 }[rank],
            "GC" => rank > 5 ? 0 : new int[] { 0, 10, 8, 6, 4, 2 }[rank],
            "Points" => rank > 5 ? 0 : new int[] { 0, 8, 6, 4, 2, 1 }[rank],
            "KOM" => rank > 5 ? 0 : new int[] { 0, 6, 4, 3, 2, 1 }[rank],
            "Youth" => rank > 3 ? 0 : new int[] { 0, 5, 3, 1 }[rank],
            _ => 0
        };

    private static int EindScore(int rank, string tab)
        => tab switch
        {
            "GC" => rank > 20 ? 0 : new int[] { 0, 100, 80, 60, 50, 40, 36, 32, 28, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 }[rank],
            "Points" => rank > 10 ? 0 : new int[] { 0, 80, 60, 40, 30, 20, 10, 8, 6, 4, 2 }[rank],
            "KOM" => rank > 5 ? 0 : new int[] { 0, 60, 40, 30, 20, 10 }[rank],
            "Youth" => rank > 5 ? 0 : new int[] { 0, 50, 30, 20, 10, 5 }[rank],
            _ => 0
        };

    private static int TeamScore(RiderResult rider, string teamWinner, string classification, StageType stageType)
    {
        if (IsLeader(rider, classification) || rider.Team != teamWinner) return 0;
        var teampointsDict = stageType switch
        {
            StageType.REG => new Dictionary<string, int>() { { "Stage", 10 }, { "GC", 8 }, { "Points", 6 }, { "KOM", 3 }, { "Youth", 2 } },
            StageType.ITT or StageType.TTT => new Dictionary<string, int>() { { "GC", 8 }, { "Points", 6 }, { "KOM", 3 }, { "Youth", 2 } },
            StageType.FinalStandings => new Dictionary<string, int>() { { "GC", 24 }, { "Points", 18 }, { "KOM", 9 }, { "Youth", 6 } },
            _ => throw new ArgumentOutOfRangeException()
        };
        return teampointsDict.TryGetValue(classification, out var teampoints) ? teampoints : 0;
    }

    private static bool IsLeader(RiderResult rider, string classification)
        => classification switch
        {
            "" or "Stage" => rider.Stagepos == 1,
            "GC" => rider.Gcpos == 1,
            "Points" => rider.Pointspos == 1,
            "KOM" => rider.Kompos == 1,
            "Youth" => rider.Yocpos == 1,
            _ => false
        };
}