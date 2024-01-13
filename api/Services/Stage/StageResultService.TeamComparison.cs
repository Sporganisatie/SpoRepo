
using Microsoft.EntityFrameworkCore;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    public IEnumerable<UserSelection> AllStageSelections(int raceId, bool budgetParticipation, int stagenr)
    {
        if (DB.Stages.Single(s => s.RaceId == raceId && s.Stagenr == stagenr).Starttime > DateTime.UtcNow) return new List<UserSelection>();

        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var users = DB.StageSelections.Include(ss => ss.AccountParticipation.Account).Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation).Select(ss => new { ss.StageSelectionId, ss.AccountParticipation.Account.Username, ss.AccountParticipationId }).ToList();
        var allSelected = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipation.BudgetParticipation == budgetParticipation && ssr.StageSelection.Stage.RaceId == raceId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                        join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                        from rp in results.DefaultIfEmpty()
                        where ssr.StageSelection.StageSelectionId == user.StageSelectionId
                        select new StageComparisonRider
                        {
                            Rider = ssr.RiderParticipation.Rider,
                            Kopman = ssr.RiderParticipationId == ssr.StageSelection.KopmanId,
                            StagePos = rp.StagePos,
                            TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.StageScore * 0.5) : 0),
                            Selected = stageSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                            // TODO dnf
                        };
            var riderScores = query.OrderBy(r => r.StagePos).ToList();

            var gemistQuery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                              join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                              from rp in results.DefaultIfEmpty()
                              let totalScore = (budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0
                              where (ts.AccountParticipationId == user.AccountParticipationId) && (totalScore > 0 || allSelected.Contains(ts.RiderParticipationId))
                                && !DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.StageSelectionId == user.StageSelectionId).Any(ssr => ssr.RiderParticipationId == ts.RiderParticipationId)
                              select new StageComparisonRider
                              {
                                  Rider = ts.RiderParticipation.Rider,
                                  StagePos = rp.StagePos,
                                  TotalScore = totalScore,
                                  Selected = stageSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                                  // TODO dnf
                              };

            output.Add(new UserSelection(user.Username, riderScores, gemistQuery.ToList()));
        }
        return OrderSelectedRiders(output).OrderByDescending(x => x.Riders.Last().TotalScore);
    }

    public IEnumerable<UserSelection> AllTeamSelections(int raceId, bool budgetParticipation) // TODO naar race service
    {
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var users = DB.AccountParticipations.Include(ap => ap.Account).Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation).Select(ss => new { ss.AccountParticipationId, ss.Account.Username }).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider).Where(ts => ts.AccountParticipationId == user.AccountParticipationId)
                        join rp in
                        (from rp in DB.ResultsPoints.Include(rp => rp.RiderParticipation.Rider)
                         join ssr in DB.StageSelectionRiders on rp.RiderParticipationId equals ssr.RiderParticipationId
                         join ss in DB.StageSelections on new { ssr.StageSelectionId, rp.StageId } equals new { StageSelectionId = ss.StageSelectionId, StageId = ss.StageId }
                         where ss.AccountParticipationId == user.AccountParticipationId
                         group new { rp, ss.KopmanId } by rp.RiderParticipation into g
                         select new
                         {
                             RiderParticipation = g.Key,
                             TotalScore = (int?)g.Sum(item => item.KopmanId == item.rp.RiderParticipationId ? item.rp.Totalscore + item.rp.StageScore * 0.5 : item.rp.Totalscore) - (budgetParticipation ? g.Sum(item => item.rp.Teamscore) : 0),
                         }) on ts.RiderParticipationId equals rp.RiderParticipation.RiderParticipationId into results
                        from rp in results.DefaultIfEmpty()
                        select new StageComparisonRider
                        {
                            Rider = ts.RiderParticipation.Rider,
                            TotalScore = rp == null ? 0 : (rp.TotalScore ?? 0),
                            Selected = teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : StageSelectedEnum.None,
                            Dnf = ts.RiderParticipation.Dnf
                        };

            var riderScores = query.ToList().OrderByDescending(x => x.TotalScore);
            output.Add(new UserSelection(user.Username, riderScores, new List<StageComparisonRider>()));
        }
        return OrderSelectedRiders(output); // Add totals again
    }

    private IEnumerable<UserSelection> OrderSelectedRiders(List<UserSelection> selecties) // TODO naar shared/helper file
    {
        var riders = selecties.SelectMany(selection => selection.Riders.Select(rider => (selection.Username, rider)))
            .GroupBy(x => x.rider.Rider).Select(g => g.ToList())
            .OrderByDescending(rider => rider.Count)
            .ThenByDescending(rider => rider.Max(u => u.Item2.TotalScore)).ToList();

        var reorderedRiders = new List<List<(string, StageComparisonRider)>>();

        while (riders.Any())
        {
            var riderLine = new List<(string, StageComparisonRider)>();
            while (riderLine.Count() < selecties.Count())
            {
                var nextRider = riders.FirstOrDefault(x => !x.Select(x => x.Item1).Intersect(riderLine.Select(x => x.Item1)).Any());
                if (nextRider is null)
                {
                    foreach (var user in selecties.Select(x => x.Username).Except(riderLine.Select(x => x.Item1)))
                    {
                        riderLine.Add(new(user, new() { TotalScore = -1 }));
                    }
                    continue;
                }
                riderLine.AddRange(nextRider);
                riders.Remove(nextRider);
            }
            reorderedRiders.Add(riderLine);
        }

        return selecties.Select(user => UpdateUser(user, reorderedRiders));
        // TODO ook return all selected riders + count
    }

    private UserSelection UpdateUser(UserSelection user, List<List<(string, StageComparisonRider)>> reorderedRiders)
    {
        var newRiders = reorderedRiders.Select(line => line.FirstOrDefault(x => x.Item1 == user.Username).Item2);
        var totals = new StageComparisonRider
        {
            TotalScore = newRiders.Sum(rs => rs.TotalScore)
        };
        return new UserSelection(user.Username, newRiders.Append(totals), user.Gemist);
    }
}