namespace SpoRE.Services;

public record RaceUitslagChart(List<UserAndTotalScore> UsernamesAndScores, int StageNumber);

public record UserAndTotalScore(string Username, double Score, int AccountId);

public partial class StatisticsService
{
    public IEnumerable<EtappeUitslag> ScoreVerloop(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections // TODO deze query bestaat nu 3x
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           Account = ss.AccountParticipation.Account,
                           TotalScore = ss.TotalScore,
                           StageNumber = ss.Stage.Stagenr
                       };

        var result = subquery
            .GroupBy(ss => ss.StageNumber)
            .Select(g => new EtappeUitslag(
                g.OrderBy(ss => ss.Account.AccountId)
                 .Select(ss => new UsernameAndScore(ss.Account.Username, ss.TotalScore - ((int)g.Average(x => x.TotalScore)) ?? 0))
                 .ToList(),
                g.Key))
            .ToList();

        var start = new EtappeUitslag(result.First().UsernamesAndScores.Select(x => new UsernameAndScore(x.Username, 0)).ToList(), 0);
        return result.Prepend(start);
    }

    public IEnumerable<EtappeUitslag> PerfectScoreVerloop(int raceId, bool budgetParticipation)
    {
        var missedPoints = MissedPoints(raceId, budgetParticipation).ToList();
        var summed = missedPoints.Select(x => x with { Data = SumMissedPointsData(x.Data) });
        var result = new List<EtappeUitslag>();
        result.Add(new EtappeUitslag(summed.Select(x => new UsernameAndScore(x.Username, 0)).ToList(), 0));
        for (int i = 0; i < summed.First().Data.Count; i++)
        {
            result.Add(new EtappeUitslag(summed.Select(x => new UsernameAndScore(x.Username, x.Data[i].Optimaal)).ToList(), i + 1));
        }
        return result.Take(result.Count() - 1).Select(x => x with { UsernamesAndScores = MinusAverage(x.UsernamesAndScores) });
    }

    public IEnumerable<UsernameAndScore> MinusAverage(IEnumerable<UsernameAndScore> input)
        => input.Select(x => x with { Score = x.Score - (int)input.Average(y => y.Score) });

    public List<MissedPointsData> SumMissedPointsData(IEnumerable<MissedPointsData> input)
    {
        var output = new List<MissedPointsData>();
        for (int i = 0; i < input.Count(); i++)
        {
            output.Add(new MissedPointsData("0", 0, input.Take(i + 1).Sum(x => x.Optimaal), 0));
        }
        return output.ToList();
    }

    private IEnumerable<RaceUitslagChart> StandPerEtappe(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           Username = ss.AccountParticipation.Account.Username,
                           TotalScore = ss.TotalScore,
                           StageNumber = ss.Stage.Stagenr,
                           AccountId = ss.AccountParticipation.AccountId
                       };

        var result = subquery
            .GroupBy(ss => ss.StageNumber)
            .Select(g => new RaceUitslagChart(
                g.OrderByDescending(ss => ss.TotalScore)
                 .Select(ss => new UserAndTotalScore(ss.Username, ss.TotalScore ?? 0, ss.AccountId))
                 .ToList(),
                g.Key))
            .ToList();

        return result;
    }

    public IEnumerable<RaceUitslagChart> PositieVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = StandPerEtappe(raceId, budgetParticipation);

        var etappeUitslagen = new List<RaceUitslagChart>();
        foreach (var uitslag in uitslagen)
        {
            var etappeUitslag = new RaceUitslagChart(new List<UserAndTotalScore>(), uitslag.StageNumber);
            var rank = 0;
            var userscores = uitslag.UsernamesAndScores.ToList();
            for (int i = 0; i < userscores.Count(); i++)
            {
                var user = userscores[i];
                if (rank == 0 || user.Score < userscores[i - 1].Score) rank++;
                etappeUitslag.UsernamesAndScores.Add(new UserAndTotalScore(user.Username, rank, user.AccountId));
            }
            etappeUitslagen.Add(etappeUitslag);
        }
        etappeUitslagen[0] = etappeUitslagen[0] with { UsernamesAndScores = etappeUitslagen[0].UsernamesAndScores.OrderBy(x => x.AccountId).ToList() };
        var startPos = etappeUitslagen.First().UsernamesAndScores.Count / 2d;
        var start = new RaceUitslagChart(etappeUitslagen.First().UsernamesAndScores.Select(x => new UserAndTotalScore(x.Username, startPos, x.AccountId)).ToList(), 0);
        return etappeUitslagen.Prepend(start);
    }
}
