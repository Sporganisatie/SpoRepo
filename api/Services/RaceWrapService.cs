
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class RaceWrapService(DatabaseContext DB)
{
    public record RaceScoreStatistic(
        int topScore,
        int bottomScore,
        int average,
        List<UserRaceScoreQueryResult> UsernamesAndScores,
        int Year,
        string Name,
        int RaceId
    );

    public IEnumerable<RaceScoreStatistic> RaceScoreStatistics(bool budgetParticipation)
    {
        var result = GetUserRaceScore(budgetParticipation)
            .GroupBy(ss => ss.Race)
            .Select(g =>
            {
                var sortedScore = g.OrderByDescending(us => us.Score.Value).ToList();

                return new RaceScoreStatistic(
                    sortedScore.First().Score.Value,
                    sortedScore.Last().Score.Value,
                    (int) sortedScore.Average(us => us.Score.Value),
                    sortedScore,
                    g.Key.Year,
                    g.Key.Name,
                    g.Key.RaceId
                );
            });

        return result;
    }

    public record UserRaceScoreQueryResult(string Username, int? Score, Race Race);

    public IEnumerable<UserRaceScoreQueryResult> GetUserRaceScore(bool budgetParticipation)
        => (from ap in DB.AccountParticipations.Include(ap => ap.Race)
            where ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics"
            select new UserRaceScoreQueryResult(
                ap.Account.Username,
                ap.FinalScore,
                ap.Race
            )).AsEnumerable();

    internal int Current()
        => DB.Races.OrderByDescending(x => x.RaceId).First(x => x.RaceId != 99).RaceId;

    internal IEnumerable<RaceSelection> AllRaces()
    {
        var query = from s in DB.Stages.Include(x => x.Race)
                    where s.RaceId != 99 && s.Race.Name != "classics"
                    group s.Race by s.Race into g
                    select g.Key;

        return query.OrderByDescending(x => x.Year).ThenByDescending(x => x.Name).ToList().Select(x => new RaceSelection($"{char.ToUpper(x.Name[0]) + x.Name[1..]} \t {x.Year}", x.RaceId));
    }
}