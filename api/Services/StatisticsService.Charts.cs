using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public record EtappeUitslagChart(List<UserAndTotalScore> UsernamesAndScores, int StageNumber);
public record RaceUitslagChart(List<UserAndTotalScore> UsernamesAndScores, int Year, string Name, int StageNumber);

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

    public IEnumerable<RaceUitslagChart> RaceScoreVerloop(bool budgetParticipation)
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

        List<(Infrastructure.Database.Account Account, int? FinalScore, Infrastructure.Database.Race Race)> modifiedList = new List<(Infrastructure.Database.Account, int?, Infrastructure.Database.Race)>();
        foreach (var user in ordered)
        {
            int sum = 0;
            foreach (var item in user.Scores)
            {
                sum += item.FinalScore ?? 0;
                modifiedList.Add((item.Account, sum, item.Race));
            }
        }

        var result = modifiedList
            .GroupBy(ss => ss.Race)
            .Select(g => new RaceUitslagChart(
                g.OrderBy(ss => ss.Account.AccountId)
                 .Select(ss => new UserAndTotalScore(ss.Account.Username, ss.FinalScore - ((int)g.Average(x => x.FinalScore)) ?? 0, ss.Account.AccountId))
                 .ToList(),
                g.Key.Year,
                char.ToUpper(g.Key.Name[0]) + g.Key.Name.Substring(1),
                g.Key.RaceId))
            .ToList().OrderBy(x => x.Year).ThenBy(x => x.Name);

        // var start = new EtappeUitslag(result.First().UsernamesAndScores.Select(x => new UsernameAndScore(x.Username, 0)).ToList(), 0);
        // return result.Prepend(start);
        return result;
    }

    private IEnumerable<EtappeUitslagChart> StandPerEtappe(int raceId, bool budgetParticipation)
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
            .Select(g => new EtappeUitslagChart(
                g.OrderByDescending(ss => ss.TotalScore)
                 .Select(ss => new UserAndTotalScore(ss.Username, ss.TotalScore ?? 0, ss.AccountId))
                 .ToList(),
                g.Key))
            .ToList();

        return result;
    }

    public IEnumerable<EtappeUitslagChart> PositieVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = StandPerEtappe(raceId, budgetParticipation);

        var etappeUitslagen = new List<EtappeUitslagChart>();
        foreach (var uitslag in uitslagen)
        {
            var etappeUitslag = new EtappeUitslagChart(new List<UserAndTotalScore>(), uitslag.StageNumber);
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
