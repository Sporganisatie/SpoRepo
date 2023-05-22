namespace SpoRE.Services;

public record RaceUitslagChart(List<UserAndTotalScore> UsernamesAndScores, int StageNumber);

public record UserAndTotalScore(string Username, int Score, int AccountId);

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
        return etappeUitslagen;
    }
}
