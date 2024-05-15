using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public record LineChartData(IEnumerable<string> Users, IEnumerable<Dictionary<string, object>> Data);

public partial class StatisticsService
{
    public LineChartData ScoreVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = (from uss in UserStageScores(raceId, budgetParticipation)
                         group uss by uss.StageNumber into stageScores
                         orderby stageScores.Key
                         select new Scores(stageScores.Select(x => new UsernameScore(x.Username, x.TotalScore.Value - (int)stageScores.Average(y => y.TotalScore))).ToList(), stageScores.Key.ToString())).ToList();

        var participants = GetParticipants(raceId, budgetParticipation);
        var start = new Scores(participants.Select(par => new UsernameScore(par, 0)).ToList(), "");

        return new(participants, uitslagen.Prepend(start).Select(x => ConvertToDict(x)));
    }

    private static Dictionary<string, object> ConvertToDict(Scores scores)
    {
        var dict = new Dictionary<string, object>
        {
            {"Name", scores.StageNumber}
        };
        foreach (var userscore in scores.UsernamesAndScores)
        {
            dict.Add(userscore.Username, userscore.Score);
        }
        return dict;
    }

    public LineChartData RaceScoreVerloop(bool budgetParticipation)
    {
        var subquery = from ap in DB.AccountParticipations.Include(ap => ap.Race)
                       where ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics" && ap.AccountId <= 5
                       select new
                       {
                           ap.Account,
                           ap.FinalScore,
                           ap.Race
                       };

        var perUser = from sq in subquery
                      group new { sq } by sq.Account into g
                      select new
                      {
                          g.Key,
                          scores = g.ToList().Select(sq => sq.sq)
                      };

        var ordered = perUser.ToList().Select(x => new { Username = x.Key, Scores = x.scores.OrderBy(x => x.Race.Year).ThenBy(x => x.Race.Name) });

        var modifiedList = new List<(Account Account, int? FinalScore, Race Race)>();
        foreach (var user in ordered)
        {
            int sum = 0;
            foreach (var item in user.Scores)
            {
                sum += item.FinalScore ?? 0;
                modifiedList.Add((item.Account, sum, item.Race));
            }
        }

        var uitslagen = modifiedList
            .GroupBy(ss => ss.Race)
            .Select(g => new Scores(
                g.OrderBy(ss => ss.Account.AccountId)
                 .Select(ss => new UsernameScore(ss.Account.Username, ss.FinalScore - ((int)g.Average(x => x.FinalScore)) ?? 0))
                 .ToList(),
                $"{g.Key.Year} {char.ToUpper(g.Key.Name[0]) + g.Key.Name[1..]}"))
            .ToList().OrderBy(x => x.StageNumber).ToList();

        var participants = uitslagen.First().UsernamesAndScores.Select(x => x.Username);

        var start = new Scores(participants.Select(x => new UsernameScore(x, 0)).ToList(), "");

        return new(participants, uitslagen.Prepend(start).Select(x => ConvertToDict(x)));
    }

    public LineChartData PerfectScoreVerloop(int raceId, bool budgetParticipation)
    {
        var missedPoints = MissedPoints(raceId, budgetParticipation).ToList();
        var summed = missedPoints.Select(x => x with { Data = SumMissedPointsData(x.Data) });
        var participants = GetParticipants(raceId, budgetParticipation);

        var result = new List<Scores>
        {
            new(participants.Select(x => new UsernameScore(x, 0)).ToList(), "0")
        };

        for (int i = 0; i < summed.First().Data.Count; i++)
        {
            result.Add(new Scores(summed.Select(x => new UsernameScore(x.Username, x.Data[i].Optimaal)).ToList(), (i + 1).ToString()));
        }

        return new(participants, result.Take(result.Count - 1).Select(x => ConvertToDict(x with { UsernamesAndScores = MinusAverage(x.UsernamesAndScores) })));
    }

    public List<UsernameScore> MinusAverage(IEnumerable<UsernameScore> input)
        => input.Select(x => x with { Score = x.Score - (int)input.Average(y => y.Score) }).ToList();

    public List<MissedPointsData> SumMissedPointsData(IEnumerable<MissedPointsData> input)
    {
        var output = new List<MissedPointsData>();
        for (int i = 0; i < input.Count(); i++)
        {
            output.Add(new MissedPointsData("0", 0, input.Take(i + 1).Sum(x => x.Optimaal), 0));
        }
        return output.ToList();
    }

    private IEnumerable<Scores> StandPerEtappe(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           ss.AccountParticipation.Account.Username,
                           ss.TotalScore,
                           ss.Stage.Stagenr,
                           ss.AccountParticipation.AccountId
                       };

        var result = subquery
            .GroupBy(ss => ss.Stagenr)
            .Select(g => new Scores(
                g.OrderByDescending(ss => ss.TotalScore)
                 .Select(ss => new UsernameScore(ss.Username, ss.TotalScore ?? 0))
                 .ToList(),
                $"{g.Key}"))
            .ToList();

        return result;
    }

    public LineChartData PositieVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = StandPerEtappe(raceId, budgetParticipation);

        var participants = GetParticipants(raceId, budgetParticipation);
        var startPos = (int)(participants.Count / -2d);
        var etappeUitslagen = new List<Scores>
        {
            new(participants.Select(x => new UsernameScore(x,startPos)).ToList(), "0")
        };

        foreach (var uitslag in uitslagen)
        {
            var etappeUitslag = new Scores([], uitslag.StageNumber);
            var rank = 0;
            var userscores = uitslag.UsernamesAndScores.ToList();
            var timesTied = 0;
            for (int i = 0; i < userscores.Count; i++)
            {
                var user = userscores[i];
                if (rank == 0 || user.Score < userscores[i - 1].Score)
                {
                    rank++;
                    rank += timesTied;
                    timesTied = 0;
                }
                else timesTied++;
                etappeUitslag.UsernamesAndScores.Add(new UsernameScore(user.Username, rank * -1));
            }
            etappeUitslagen.Add(etappeUitslag);
        }
        return new(participants, etappeUitslagen.Select(x => ConvertToDict(x)));
    }

    private List<string> GetParticipants(int raceId, bool budgetParticipation)
        => DB.AccountParticipations.Include(x => x.Account)
            .Where(x => x.RaceId == raceId && x.BudgetParticipation == budgetParticipation)
            .OrderBy(x => x.AccountId)
            .Select(x => x.Account.Username).ToList();
}