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
                        StagePos = rp.Stagepos,
                        StageScore = (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.Stagescore * 1.5) : rp.Stagescore) ?? 0,
                        ClassificationScore = rp.Gcscore + rp.Pointsscore + rp.Komscore + rp.Yocscore ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.Stagescore * 0.5) : 0)
                    };

        var finalquery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                         join rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                         from rp in results.DefaultIfEmpty()
                         where ts.AccountParticipationId == User.ParticipationId
                         select new RiderScore
                         {
                             Rider = ts.RiderParticipation.Rider,
                             StagePos = rp.Stagepos,
                             StageScore = rp.Stagescore ?? 0,
                             ClassificationScore = rp.Gcscore + rp.Pointsscore + rp.Komscore + rp.Yocscore ?? 0,
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

        var stageResult = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Stagepos > 0).OrderBy(rp => rp.Stagepos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Team = rp.RiderParticipation.Team,
                              Position = rp.Stagepos,
                              Result = rp.Stageresult,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var gcStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Gcpos > 0).OrderBy(rp => rp.Gcpos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Team = rp.RiderParticipation.Team,
                              Position = rp.Gcpos,
                              Result = rp.Gcresult,
                              Change = rp.Gcchange,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var pointsStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Pointspos > 0).OrderBy(rp => rp.Pointspos)
                              select new ClassificationRow
                              {
                                  Rider = rp.RiderParticipation.Rider,
                                  Team = rp.RiderParticipation.Team,
                                  Position = rp.Pointspos,
                                  Result = rp.Pointsresult,
                                  Change = rp.Pointschange,
                                  Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                              };

        var komStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Kompos > 0).OrderBy(rp => rp.Kompos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Team = rp.RiderParticipation.Team,
                               Position = rp.Kompos,
                               Result = rp.Komresult,
                               Change = rp.Komchange,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        var yocStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId && rp.Yocpos > 0).OrderBy(rp => rp.Yocpos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Team = rp.RiderParticipation.Team,
                               Position = rp.Yocpos,
                               Result = rp.Yocresult,
                               Change = rp.Yocchange,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        return top5 ? new(gcStandings.Take(5).ToList(), pointsStandings.Take(5).ToList(), komStandings.Take(5).ToList(), yocStandings.Take(5).ToList())
                : new(gcStandings.ToList(), pointsStandings.ToList(), komStandings.ToList(), yocStandings.ToList()) { Stage = stageResult.ToList() };
    }
}