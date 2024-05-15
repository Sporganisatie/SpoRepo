namespace SpoRE.Services;

public record EtappeUitslagen(IEnumerable<Scores> uitslagen, IEnumerable<ScoreVerdeling> scoreVerdeling, IEnumerable<UserRank> userRanks);

public record Scores(List<UsernameScore> UsernamesAndScores, string StageNumber);

public record UsernameScore(string Username, int Score);

public record ScoreVerdeling(string Username, int Bin0, int Bin1, int Bin2, int Bin3, int Bin4);

public record UserRank(string Username, int[] Ranks);

public partial class StatisticsService
{
    public EtappeUitslagen EtappeUitslagen(int raceId, bool budgetParticipation)
    {
        var uitslagen = SortedUitslagen(raceId, budgetParticipation);
        var scoreVerdeling = ScoreVerdeling(raceId, budgetParticipation);
        var userRank = CountRanks(uitslagen.Select(x => x.UsernamesAndScores), uitslagen.First().UsernamesAndScores.Select(x => x.Username));
        return new(uitslagen, scoreVerdeling, userRank);
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

    private record StageSelectionQueryResult(string Username, int? StageScore, int? TotalScore, int StageNumber);

    private IEnumerable<StageSelectionQueryResult> UserStageScores(int raceId, bool budgetParticipation)
        => (from ss in DB.StageSelections
            where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
            orderby ss.AccountParticipation.AccountId
            select new StageSelectionQueryResult(
                ss.AccountParticipation.Account.Username,
                ss.StageScore,
                ss.TotalScore,
                ss.Stage.Stagenr)).AsEnumerable();
}
