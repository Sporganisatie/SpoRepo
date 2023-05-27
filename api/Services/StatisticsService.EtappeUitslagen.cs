namespace SpoRE.Services;

public record EtappeUitslagen(IEnumerable<EtappeUitslag> uitslagen, IEnumerable<ScoreVerdeling> scoreVerdeling, IEnumerable<UserRank> userRanks);

public record EtappeUitslag(IEnumerable<UsernameAndScore> UsernamesAndScores, int StageNumber);

public record UsernameAndScore(string Username, int Score);

public record ScoreVerdeling(string Username, int Bin0, int Bin1, int Bin2, int Bin3, int Bin4);

public record UserRank(string Username, int[] Ranks);

public partial class StatisticsService
{
    public EtappeUitslagen EtappeUitslagen(int raceId, bool budgetParticipation)
    {
        var uitslagen = SortedUitslagen(raceId, budgetParticipation);
        var scoreVerdeling = ScoreVerdeling(raceId, budgetParticipation);
        var userRank = UserRankCounts(uitslagen);
        return new(uitslagen, scoreVerdeling, userRank);
    }

    private IEnumerable<UserRank> UserRankCounts(IEnumerable<EtappeUitslag> uitslagen)
    {
        var users = new Dictionary<string, int[]>();
        foreach (UsernameAndScore item in uitslagen.First().UsernamesAndScores)
        {
            users[item.Username] = new int[uitslagen.First().UsernamesAndScores.Count()];
        }

        foreach (var uitslag in uitslagen)
        {
            var rank = 0;
            var userscores = uitslag.UsernamesAndScores.ToList();
            for (int i = 0; i < userscores.Count(); i++)
            {
                var user = userscores[i];
                if (rank == 0 || user.Score < userscores[i - 1].Score) rank++;
                users[user.Username][rank - 1]++;
            }
        }
        return users.Select(x => new UserRank(x.Key, x.Value));
    }

    public IEnumerable<EtappeUitslag> SortedUitslagen(int raceId, bool budgetParticipation)
        => Uitslagen(raceId, budgetParticipation)
            .Select(x => new EtappeUitslag(x.UsernamesAndScores.OrderByDescending(y => y.Score), x.StageNumber));

    public IEnumerable<EtappeUitslag> Uitslagen(int raceId, bool budgetParticipation)
        => from uss in UserStageScores(raceId, budgetParticipation)
           group uss by uss.StageNumber into stageScores
           orderby stageScores.Key
           select new EtappeUitslag(stageScores.Select(x => new UsernameAndScore(x.Username, x.StageScore ?? 0)).ToList(), stageScores.Key);

    private IEnumerable<ScoreVerdeling> ScoreVerdeling(int raceId, bool budgetParticipation)
    {
        var bins = budgetParticipation ? new[] { 0, 10, 30, 50, 100 } : new[] { 0, 50, 100, 200, 300 };

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

        return result.ToList();
    }

    private record StageSelectionQueryResult(string Username, int? StageScore, int StageNumber);

    private IEnumerable<StageSelectionQueryResult> UserStageScores(int raceId, bool budgetParticipation)
        => (from ss in DB.StageSelections
            where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
            orderby ss.AccountParticipation.AccountId
            select new StageSelectionQueryResult(
                ss.AccountParticipation.Account.Username,
                ss.StageScore,
                ss.Stage.Stagenr)).AsEnumerable();
}
