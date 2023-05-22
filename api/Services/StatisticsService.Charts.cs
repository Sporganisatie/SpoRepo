namespace SpoRE.Services;

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
}
