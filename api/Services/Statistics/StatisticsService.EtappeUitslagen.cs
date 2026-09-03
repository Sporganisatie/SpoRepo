namespace SpoRE.Services;

public record EtappeUitslagen(
    IEnumerable<Scores> uitslagen,
    IEnumerable<ScoreVerdeling> scoreVerdeling,
    IEnumerable<UserRank> userRanks,
    IEnumerable<UserStageScores> stageScoreSpread);

public record Scores(List<UsernameScore> UsernamesAndScores, string StageNumber);

public record UsernameScore(string Username, int Score);

public record ScoreVerdeling(string Username, int Bin0, int Bin1, int Bin2, int Bin3, int Bin4);

public record UserRank(string Username, int[] Ranks);

public record UserStageScores(string Username, List<int> StageScores);

public partial class StatisticsService
{
    public EtappeUitslagen EtappeUitslagen(int raceId, bool budgetParticipation)
    {
        // Meeste etappe winsten per ronde
        // var races = DB.Races.Where(r => r.Name != "classics").OrderBy(r => r.Year).ThenBy(r => r.Name).ToList();
        // var prints = new List<(string, int)>();
        // foreach (var race in races)
        // {
        //     var uitslagen2 = SortedUitslagen(race.RaceId, budgetParticipation);
        //     if (!uitslagen2.Any()) continue;
        //     var userRank2 = CountRanks(uitslagen2.Select(x => x.UsernamesAndScores), uitslagen2.First().UsernamesAndScores.Select(x => x.Username));
        //     var beste = userRank2.First();
        //     prints.Add(($"{race.Name} {race.Year} {beste.Username} {beste.Ranks[0]}", beste.Ranks[0]));
        // }
        // Console.WriteLine("HIERRRRRRRRRRRRRRR");
        // foreach (var item in prints.OrderByDescending(x => x.Item2))
        // {
        //     Console.WriteLine(item.Item1);
        // }

        var uitslagen = SortedUitslagen(raceId, budgetParticipation);
        var scoreVerdeling = ScoreVerdeling(raceId, budgetParticipation);
        var stageScoreSpread = StageScoreSpread(raceId, budgetParticipation);
        var usernames = uitslagen.FirstOrDefault()?.UsernamesAndScores.Select(x => x.Username) ?? stageScoreSpread.Select(x => x.Username);
        var userRank = CountRanks(uitslagen.Select(x => x.UsernamesAndScores), usernames);
        return new(uitslagen, scoreVerdeling, userRank, stageScoreSpread);
    }

    public IEnumerable<Scores> SortedUitslagen(int raceId, bool budgetParticipation)
        => Uitslagen(raceId, budgetParticipation)
            .Select(x => new Scores(x.UsernamesAndScores.OrderByDescending(y => y.Score).ToList(), x.StageNumber));

    public List<Scores> Uitslagen(int raceId, bool budgetParticipation)
        => (from uss in UserStageScores(raceId, budgetParticipation)
            group uss by uss.StageNumber into stageScores
            orderby stageScores.Key
            select new Scores(stageScores.Select(x => new UsernameScore(x.Username, x.StageScore ?? 0)).ToList(), stageScores.Key.ToString())).ToList();

    private IEnumerable<ScoreVerdeling> ScoreVerdeling(int raceId, bool budgetParticipation)
    {
        var bins = budgetParticipation ? new[] { 0, 10, 30, 50, 100 } : [0, 50, 100, 200, 300];

        var result = from item in UserStageScores(raceId, budgetParticipation)
                     group item by item.Username into userGroup
                     select new ScoreVerdeling
                     (
                         userGroup.Key,
                         userGroup.Count(item => item.StageScore >= bins[0] && item.StageScore < bins[1]),
                         userGroup.Count(item => item.StageScore >= bins[1] && item.StageScore < bins[2]),
                         userGroup.Count(item => item.StageScore >= bins[2] && item.StageScore < bins[3]),
                         userGroup.Count(item => item.StageScore >= bins[3] && item.StageScore < bins[4]),
                         userGroup.Count(item => item.StageScore >= bins[4]));

        return result.ToList().OrderByDescending(x => x.Bin4).ThenByDescending(x => x.Bin3).ThenByDescending(x => x.Bin2).ThenByDescending(x => x.Bin1).ThenByDescending(x => x.Bin0);
    }

    private IEnumerable<UserStageScores> StageScoreSpread(int raceId, bool budgetParticipation)
    {
        var stageScores = UserStageScores(raceId, budgetParticipation).ToList();

        return stageScores
            .GroupBy(x => x.Username)
            .OrderBy(g => g.Min(x => x.AccountId))
            .Select(g => new UserStageScores(
                g.Key,
                g.OrderBy(x => x.StageNumber).Select(x => x.StageScore ?? 0).ToList()))
            .ToList();
    }

    private record StageSelectionQueryResult(string Username, int? StageScore, int? TotalScore, int StageNumber, int AccountId);

    private IEnumerable<StageSelectionQueryResult> UserStageScores(int raceId, bool budgetParticipation)
        => (from ss in DB.StageSelections
            where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
            orderby ss.AccountParticipation.AccountId
            select new StageSelectionQueryResult(
                ss.AccountParticipation.Account.Username,
                ss.StageScore,
                ss.TotalScore,
                ss.Stage.Stagenr,
                ss.AccountParticipation.AccountId)).AsEnumerable();
}
