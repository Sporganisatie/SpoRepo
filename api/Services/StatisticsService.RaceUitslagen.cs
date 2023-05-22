using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public record RaceUitslagen(IEnumerable<RaceUitslag> uitslagen, IEnumerable<ScoreVerdeling> scoreVerdeling, IEnumerable<UserRank> userRanks);

public record RaceUitslag(List<UsernameAndScore> UsernamesAndScores, int Year, string Name, int StageNumber);

public partial class StatisticsService
{
    public RaceUitslagen RaceUitslagenAll(bool budgetParticipation)
    {
        var uitslagen = RaceUitslagen(budgetParticipation);
        var scoreVerdeling = RaceScoreVerdeling(budgetParticipation);
        var userRank = RaceUserRankCounts(uitslagen);
        return new(uitslagen, scoreVerdeling, userRank);
    }

    private IEnumerable<UserRank> RaceUserRankCounts(IEnumerable<RaceUitslag> uitslagen)
    {
        var users = new Dictionary<string, int[]>();
        var uniqueUsernames = uitslagen
            .SelectMany(etappe => etappe.UsernamesAndScores)
            .Select(usernameAndScore => usernameAndScore.Username)
            .Distinct()
            .ToList();
        foreach (var name in uniqueUsernames)
        {
            users[name] = new int[uniqueUsernames.Count];
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

    private IEnumerable<RaceUitslag> RaceUitslagen(bool budgetParticipation)
    {
        var subquery = from ap in DB.AccountParticipations.Include(ap => ap.Race)
                       where ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics"
                       select new
                       {
                           Username = ap.Account.Username,
                           StageScore = ap.Finalscore,
                           Race = ap.Race
                       };

        var result = subquery
            .GroupBy(ss => ss.Race)
            .Select(g => new RaceUitslag(
                g.OrderByDescending(ss => ss.StageScore)
                 .Select(ss => new UsernameAndScore(ss.Username, ss.StageScore ?? 0))
                 .ToList(),
                g.Key.Year,
                char.ToUpper(g.Key.Name[0]) + g.Key.Name.Substring(1),
                g.Key.RaceId))
            .ToList().OrderBy(x => x.Year).ThenBy(x => x.Name);

        return result;
    }

    private IEnumerable<ScoreVerdeling> RaceScoreVerdeling(bool budgetParticipation)
    {
        var subquery = from ap in DB.AccountParticipations
                       where ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics"
                       select new
                       {
                           Username = ap.Account.Username,
                           StageScore = ap.Finalscore,
                           StageNumber = ap.RaceId
                       };
        var bins = budgetParticipation ? new[] { 0, 500, 750, 100 } : new[] { 0, 4000, 4500, 5000 };

        var result = from item in subquery
                     group item by item.Username into userGroup
                     select new ScoreVerdeling
                     (
                         userGroup.Key,
                         userGroup.Count(item => item.StageScore >= bins[0] && item.StageScore < bins[1]),
                         userGroup.Count(item => item.StageScore >= bins[1] && item.StageScore < bins[2]),
                         userGroup.Count(item => item.StageScore >= bins[2] && item.StageScore < bins[3]),
                         userGroup.Count(item => item.StageScore >= bins[3]),
                         0);

        return result.ToList().OrderByDescending(x => x.Bin3).ThenByDescending(x => x.Bin2).ThenByDescending(x => x.Bin1).ThenByDescending(x => x.Bin0);
    }
}