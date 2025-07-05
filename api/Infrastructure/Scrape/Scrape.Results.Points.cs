using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private static int Score(int rank, string tab, StageType type)
        => type is StageType.FinalStandings ? EindScore(rank, tab) : StageScore(rank, tab, type);

    private static int StageScore(int rank, string tab, StageType type)
        => tab switch
        {
            "" or PcsStage => type is StageType.TTT
                ? rank > 8 ? 0 : new int[] { 0, 40, 32, 28, 24, 20, 16, 12, 8 }[rank]
                : rank > 20 ? 0 : new int[] { 0, 50, 44, 40, 36, 32, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 }[rank],
            PcsGc => rank > 5 ? 0 : new int[] { 0, 10, 8, 6, 4, 2 }[rank],
            PcsPoints => rank > 5 ? 0 : new int[] { 0, 8, 6, 4, 2, 1 }[rank],
            PcsKom => rank > 5 ? 0 : new int[] { 0, 6, 4, 3, 2, 1 }[rank],
            PcsYouth => rank > 3 ? 0 : new int[] { 0, 5, 3, 1 }[rank],
            _ => 0
        };

    private static int EindScore(int rank, string tab)
        => tab switch
        {
            PcsGc => rank > 20 ? 0 : new int[] { 0, 100, 80, 60, 50, 40, 36, 32, 28, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 }[rank],
            PcsPoints => rank > 10 ? 0 : new int[] { 0, 80, 60, 40, 30, 20, 10, 8, 6, 4, 2 }[rank],
            PcsKom => rank > 5 ? 0 : new int[] { 0, 60, 40, 30, 20, 10 }[rank],
            PcsYouth => rank > 5 ? 0 : new int[] { 0, 50, 30, 20, 10, 5 }[rank],
            _ => 0
        };

    private static int TeamScore(RiderResult rider, string teamWinner, string classification, StageType stageType)
    {
        if (IsLeader(rider, classification) || rider.Team != teamWinner) return 0;
        var teampointsDict = stageType switch
        {
            StageType.REG => new Dictionary<string, int>() { { PcsStage, 10 }, { PcsGc, 8 }, { PcsPoints, 6 }, { PcsKom, 3 }, { PcsYouth, 2 } },
            StageType.ITT or StageType.TTT => new Dictionary<string, int>() { { PcsGc, 8 }, { PcsPoints, 6 }, { PcsKom, 3 }, { PcsYouth, 2 } },
            StageType.FinalStandings => new Dictionary<string, int>() { { PcsGc, 24 }, { PcsPoints, 18 }, { PcsKom, 9 }, { PcsYouth, 6 } },
            _ => throw new ArgumentOutOfRangeException()
        };
        return teampointsDict.TryGetValue(classification, out var teampoints) ? teampoints : 0;
    }

    private static bool IsLeader(RiderResult rider, string classification)
        => classification switch
        {
            "" or PcsStage => rider.Stagepos == 1,
            PcsGc => rider.Gcpos == 1,
            PcsPoints => rider.Pointspos == 1,
            PcsKom => rider.Kompos == 1,
            PcsYouth => rider.Yocpos == 1,
            _ => false
        };
}