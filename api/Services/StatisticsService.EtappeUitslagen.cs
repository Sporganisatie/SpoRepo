namespace SpoRE.Services;

public record EtappeUitslagen(IEnumerable<EtappeUitslag> uitslagen, IEnumerable<EtappeUitslag> scoreVerdeling);

public record EtappeUitslag(List<UsernameAndScore> UsernamesAndScores, int StageNumber);

public record UsernameAndScore(string Username, int Score);

public partial class StatisticsService
{
    public EtappeUitslagen EtappeUitslagen(int raceId, bool budgetParticipation)
    {
        var uitslagen = Uitslagen(raceId, budgetParticipation);
        var scoreVerdeling = ScoreVerdeling(raceId, budgetParticipation);
        return new(uitslagen, scoreVerdeling);
    }

    private IEnumerable<EtappeUitslag> Uitslagen(int raceId, bool budgetParticipation)
    {
        var subquery = (from ss in DB.StageSelections
                        where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                        select new
                        {
                            Username = ss.AccountParticipation.Account.Username,
                            StageScore = ss.StageScore,
                            StageNumber = ss.Stage.Stagenr
                        });

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

    private IEnumerable<EtappeUitslag> ScoreVerdeling(int raceId, bool budgetParticipation)
    {
        var subquery = (from ss in DB.StageSelections
                        where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                        select new
                        {
                            Username = ss.AccountParticipation.Account.Username,
                            StageScore = ss.StageScore,
                            StageNumber = ss.Stage.Stagenr
                        });

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

}
