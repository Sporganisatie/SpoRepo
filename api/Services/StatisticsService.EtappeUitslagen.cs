namespace SpoRE.Services;

public record EtappeUitslagen(IEnumerable<EtappeUitslag> uitslagen, IEnumerable<ScoreVerdeling> scoreVerdeling, IEnumerable<UserRank> userRanks);

public record EtappeUitslag(List<UsernameAndScore> UsernamesAndScores, int StageNumber);

public record UsernameAndScore(string Username, int Score);

public record ScoreVerdeling(string Username, int Bin0, int Bin1, int Bin2, int Bin3, int Bin4);

public record UserRank(string Username, int[] Ranks);

public partial class StatisticsService
{
    public EtappeUitslagen EtappeUitslagen(int raceId, bool budgetParticipation)
    {
        var uitslagen = Uitslagen(raceId, budgetParticipation);
        var scoreVerdeling = ScoreVerdeling(raceId, budgetParticipation);
        var userRank = UserRankCounts(uitslagen);
        return new(uitslagen, scoreVerdeling, userRank);
    }

    private IEnumerable<UserRank> UserRankCounts(IEnumerable<EtappeUitslag> uitslagen)
    {
        var users = new Dictionary<string, int[]>();
        int length = uitslagen.First().UsernamesAndScores.Count;
        foreach (UsernameAndScore item in uitslagen.First().UsernamesAndScores)
        {
            users[item.Username] = new int[length];
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

    private IEnumerable<EtappeUitslag> Uitslagen(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           Username = ss.AccountParticipation.Account.Username,
                           StageScore = ss.StageScore,
                           StageNumber = ss.Stage.Stagenr
                       };

        var result = subquery
            .GroupBy(ss => ss.StageNumber)
            .Select(g => new EtappeUitslag(
                g.OrderByDescending(ss => ss.StageScore)
                 .Select(ss => new UsernameAndScore(ss.Username, ss.StageScore ?? 0))
                 .ToList(),
                g.Key))
            .ToList();

        return result;
    }

    private IEnumerable<ScoreVerdeling> ScoreVerdeling(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           Username = ss.AccountParticipation.Account.Username,
                           StageScore = ss.StageScore,
                           StageNumber = ss.Stage.Stagenr
                       };
        var bins = budgetParticipation ? new[] { 0, 10, 30, 50, 100 } : new[] { 0, 50, 100, 200, 300 };

        var result = from item in subquery
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
}
