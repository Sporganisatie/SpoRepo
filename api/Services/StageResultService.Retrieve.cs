using Microsoft.EntityFrameworkCore;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    public IEnumerable<RiderScore> GetTeamResult(int raceId, int stagenr, bool budgetParticipation)
    {
        var riderScores = GetRiderScores(raceId, stagenr, budgetParticipation).ToList();

        var totals = new RiderScore
        {
            StageScore = riderScores.Sum(rs => rs.StageScore),
            ClassificationScore = riderScores.Sum(rs => rs.ClassificationScore),
            TeamScore = riderScores.Sum(rs => rs.TeamScore),
            TotalScore = riderScores.Sum(rs => rs.TotalScore)
        };
        return riderScores.Append(totals);
    }

    private IEnumerable<RiderScore> GetRiderScores(int raceId, int stagenr, bool budgetParticipation)
    {
        var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                    join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                    from rp in results.DefaultIfEmpty()
                    where ssr.StageSelection.Stage.Stagenr == stagenr && ssr.StageSelection.AccountParticipationId == User.ParticipationId
                    select new RiderScore
                    {
                        Rider = ssr.RiderParticipation.Rider,
                        Kopman = ssr.RiderParticipationId == ssr.StageSelection.KopmanId,
                        StagePos = rp.Stagepos,
                        StageScore = (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 1.5) : rp.Stagescore) ?? 0,
                        ClassificationScore = rp.Gcscore + rp.Pointsscore + rp.Komscore + rp.Yocscore ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 0.5) : 0)
                    };

        return query.ToList().OrderByDescending(rc => rc.TotalScore).ThenBy(rc => rc.StagePos);
    }

    public IEnumerable<UserScore> GetUserScores(int raceId, bool budgetParticipation, int stagenr)
        => DB.StageSelections.Where(ss => ss.Stage.RaceId == raceId && ss.Stage.Stagenr == stagenr)
            .Join(
                DB.AccountParticipations.Where(ap => ap.Budgetparticipation == budgetParticipation),
                ss => ss.AccountParticipationId,
                ap => ap.AccountParticipationId,
                (ss, ap) => new UserScore(ap.Account, ss.Stagescore ?? 0, ss.Totalscore ?? 0)
            ).ToList().OrderByDescending(us => us.totalscore).ThenByDescending(us => us.stagescore);

    public Classifications GetClassifications(int raceId, int stagenr, bool top5)
    {
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();

        var mostRecentStage = DB.Stages.OrderByDescending(s => s.Stagenr).First(s => s.Finished && s.RaceId == raceId);

        var stageResult = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Stagepos > 0).OrderBy(rp => rp.Stagepos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Position = rp.Stagepos,
                              Result = rp.Stageresult,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var gcStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Gcpos > 0).OrderBy(rp => rp.Gcpos)
                          select new ClassificationRow
                          {
                              Rider = rp.RiderParticipation.Rider,
                              Position = rp.Gcpos,
                              Result = rp.Gcresult,
                              Change = rp.Gcchange,
                              Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                          };

        var pointsStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Pointspos > 0).OrderBy(rp => rp.Pointspos)
                              select new ClassificationRow
                              {
                                  Rider = rp.RiderParticipation.Rider,
                                  Position = rp.Pointspos,
                                  Result = rp.Pointsresult,
                                  Change = rp.Pointschange,
                                  Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                              };

        var komStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Kompos > 0).OrderBy(rp => rp.Kompos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Position = rp.Kompos,
                               Result = rp.Komresult,
                               Change = rp.Komchange,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        var yocStandings = from rp in DB.ResultsPoints.Where(rp => rp.StageId == mostRecentStage.StageId && rp.Yocpos > 0).OrderBy(rp => rp.Yocpos)
                           select new ClassificationRow
                           {
                               Rider = rp.RiderParticipation.Rider,
                               Position = rp.Yocpos,
                               Result = rp.Yocresult,
                               Change = rp.Yocchange,
                               Selected = stageSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(rp.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                           };

        return top5 ? new(gcStandings.Take(5).ToList(), pointsStandings.Take(5).ToList(), komStandings.Take(5).ToList(), yocStandings.Take(5).ToList())
                : new(gcStandings.ToList(), pointsStandings.ToList(), komStandings.ToList(), yocStandings.ToList()) { Stage = stageResult.ToList() };
    }
}