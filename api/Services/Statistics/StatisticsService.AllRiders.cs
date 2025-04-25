using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> AllRiders(int raceId, bool budgetParticipation)
    {
        var allRiderParticipations = DB.RiderParticipations
                        .Include(rp => rp.Rider)
                        .Join(DB.ResultsPoints,
                        x => x.RiderParticipationId,
                        x => x.RiderParticipationId,
                        (x, y) => new
                        {
                            RiderParticipation = x,
                            ResultsPoints = y
                        })
                        .Where(rp => rp.RiderParticipation.RaceId == raceId && rp.RiderParticipation.Price <= (budgetParticipation ? 750000 : int.MaxValue))
                        .GroupBy(rp => rp.RiderParticipation).ToList()
        .Select(rp => new
        {
            RiderParticipation = rp.Key,
            rp.Key.Rider,
            StageScore = rp.Key.ResultsPoints.Sum(x => x.StageScore),
            Klassementen = rp.Key.ResultsPoints.Sum(x => x.Gc.Score),
            TeamScore = rp.Key.ResultsPoints.Sum(x => x.Teamscore),
            TotalScore = budgetParticipation ? rp.Key.ResultsPoints.Sum(x => x.Totalscore - x.Teamscore) : rp.Key.ResultsPoints.Sum(x => x.Totalscore),
        });

        var userSelectionCounts = DB.TeamSelections
            .Include(ts => ts.AccountParticipation.Account)
            .Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation && ts.AccountParticipation.RaceId == raceId)
            .GroupBy(ts => ts.RiderParticipationId)
            .Select(g => new { RiderParticipationId = g.Key, TotalSelected = g.Count(), Accounts = g.Select(ts => ts.AccountParticipation.Account.Username).ToList() }).ToList()
            ;

        var q3 = from rp in allRiderParticipations
                 join usc in userSelectionCounts
                 on rp.RiderParticipation.RiderParticipationId equals usc.RiderParticipationId into gj
                 from usc in gj.DefaultIfEmpty()
                 select new
                 {
                     rp.RiderParticipation,
                     rp.Rider,
                     rp.StageScore,
                     rp.Klassementen,
                     rp.TeamScore,
                     rp.TotalScore,
                     TotalSelected = usc != null ? usc.TotalSelected : 0,
                     Accounts = usc != null ? usc.Accounts : new List<string>()
                 };

        // var q2 = allRiderParticipations.Join(userSelectionCounts,
        //     rp => rp.RiderParticipationId,
        //     usc => usc.RiderParticipationId,
        //     (rp, usc) => new
        //     {
        //         RiderParticipation = rp,
        //         rp.Rider,
        //         StageScore = rp.ResultsPoints.Sum(x => x.StageScore),
        //         Klassementen = rp.ResultsPoints.Sum(x => x.Gc.Score),
        //         TeamScore = rp.ResultsPoints.Sum(x => x.Teamscore),
        //         TotalScore = budgetParticipation ? rp.ResultsPoints.Sum(x => x.Totalscore - x.Teamscore) : rp.ResultsPoints.Sum(x => x.Totalscore),
        //         TotalSelected = usc != null ? usc.TotalSelected : 0,
        //         Accounts = usc != null ? usc.Accounts : new List<string>()
        //     });


        // var query = allRiderParticipations.GroupJoin(userSelectionCounts,
        //     rp => rp.RiderParticipationId,
        //     usc => usc.RiderParticipationId,
        //     (rp, usc) => new
        //     {
        //         RiderParticipation = rp,
        //         rp.Rider,
        //         StageScore = rp.ResultsPoints.Sum(x => x.StageScore),
        //         Klassementen = rp.ResultsPoints.Sum(x => x.Gc.Score),
        //         TeamScore = rp.ResultsPoints.Sum(x => x.Teamscore),
        //         TotalScore = budgetParticipation ? rp.ResultsPoints.Sum(x => x.Totalscore - x.Teamscore) : rp.ResultsPoints.Sum(x => x.Totalscore),
        //         TotalSelected = usc.FirstOrDefault() != null ? usc.FirstOrDefault().TotalSelected : 0,
        //         Accounts = usc.FirstOrDefault() != null ? usc.FirstOrDefault().Accounts : new List<string>()
        //     });

        return q3.OrderByDescending(x => x.TotalScore).ToList();
        // return userSelectionCounts;
    }
}
