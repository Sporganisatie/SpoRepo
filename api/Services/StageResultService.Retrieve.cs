using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    public IEnumerable<RiderScore> GetTeamResult(Stage stage, bool budgetParticipation)
    {
        var riderScores = GetRiderScores(stage, budgetParticipation).ToList();

        var totals = new RiderScore
        {
            StageScore = riderScores.Sum(rs => rs.StageScore),
            ClassificationScore = riderScores.Sum(rs => rs.ClassificationScore),
            TeamScore = riderScores.Sum(rs => rs.TeamScore),
            TotalScore = riderScores.Sum(rs => rs.TotalScore)
        };
        return riderScores.Append(totals);
    }

    private IEnumerable<RiderScore> GetRiderScores(Stage stage, bool budgetParticipation)
    {
        var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                    join rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                    from rp in results.DefaultIfEmpty()
                    where ssr.StageSelection.StageId == stage.StageId && ssr.StageSelection.AccountParticipationId == User.ParticipationId
                    select new RiderScore
                    {
                        Rider = ssr.RiderParticipation.Rider,
                        Kopman = ssr.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0),
                        StagePos = rp.Day.Pos,
                        StageScore = (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.Day.Score * 1.5) : rp.Day.Score) ?? 0,
                        ClassificationScore = rp.Gc.Score + rp.Points.Score + rp.Kom.Score + rp.Youth.Score ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.Day.Score * 0.5) : 0)
                    };

        var finalquery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                         join rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                         from rp in results.DefaultIfEmpty()
                         where ts.AccountParticipationId == User.ParticipationId
                         select new RiderScore
                         {
                             Rider = ts.RiderParticipation.Rider,
                             StagePos = rp.Day.Pos,
                             StageScore = rp.Day.Score ?? 0,
                             ClassificationScore = rp.Gc.Score + rp.Points.Score + rp.Kom.Score + rp.Youth.Score ?? 0,
                             TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                             TotalScore = (budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0
                         };

        var actualQuery = stage.IsFinalStandings ? finalquery : query;

        return actualQuery.ToList().OrderByDescending(rc => rc.TotalScore).ThenBy(rc => rc.StagePos);
    }

    public IEnumerable<UserScore> GetUserScores(Stage stage, bool budgetParticipation)
        => (from ss in DB.StageSelections.Where(ss => ss.StageId == stage.StageId && ss.AccountParticipation.BudgetParticipation == budgetParticipation)
            select new UserScore(ss.AccountParticipation.Account, ss.StageScore ?? 0, ss.TotalScore ?? 0))
            .ToList().OrderByDescending(us => us.totalscore).ThenByDescending(us => us.stagescore);

    public Classifications GetClassifications(Stage stage, bool top5)
    {
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var stageSelection = stage.IsFinalStandings
            ? teamSelection
            : DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.StageId == stage.StageId).Select(ssr => ssr.RiderParticipationId).ToList();

        var stageResult = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Day.Pos > 0).OrderBy(rp => rp.Day.Pos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Team = rp.RiderParticipation.Team,
                              Result = rp.Day,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var gcStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Gc.Pos > 0).OrderBy(rp => rp.Gc.Pos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Team = rp.RiderParticipation.Team,
                              Result = rp.Gc,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var pointsStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Points.Pos > 0).OrderBy(rp => rp.Points.Pos)
                              select new ClassificationRow
                              {
                                  Rider = rp.RiderParticipation.Rider,
                                  Team = rp.RiderParticipation.Team,
                                  Result = rp.Points,
                                  Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                              };

        var komStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Kom.Pos > 0).OrderBy(rp => rp.Kom.Pos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Team = rp.RiderParticipation.Team,
                               Result = rp.Kom,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        var youthStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Youth.Pos > 0).OrderBy(rp => rp.Youth.Pos)
                             select new ClassificationRow
                             {
                                 Rider = rp.RiderParticipation.Rider,
                                 Team = rp.RiderParticipation.Team,
                                 Result = rp.Youth,
                                 Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                             };

        return top5 ? new(gcStandings.Take(5).ToList(), pointsStandings.Take(5).ToList(), komStandings.Take(5).ToList(), youthStandings.Take(5).ToList())
                : new(gcStandings.ToList(), pointsStandings.ToList(), komStandings.ToList(), youthStandings.ToList()) { Stage = stageResult.ToList() };
    }
}