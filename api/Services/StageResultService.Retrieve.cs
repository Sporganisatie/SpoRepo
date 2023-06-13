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
                        StagePos = rp.StagePos,
                        StageScore = (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.StageScore * 1.5) : rp.StageScore) ?? 0,
                        ClassificationScore = rp.Gc.Score + rp.Points.Score + rp.Kom.Score + rp.Youth.Score ?? 0,
                        TeamScore = budgetParticipation ? 0 : rp.Teamscore ?? 0,
                        TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == (ssr.StageSelection.KopmanId ?? 0) ? (int)(rp.StageScore * 0.5) : 0)
                    };

        var finalquery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                         join rp in DB.ResultsPoints.Where(rp => rp.StageId == stage.StageId) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                         from rp in results.DefaultIfEmpty()
                         where ts.AccountParticipationId == User.ParticipationId
                         select new RiderScore
                         {
                             Rider = ts.RiderParticipation.Rider,
                             StagePos = rp.StagePos,
                             StageScore = rp.StageScore ?? 0,
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
        var riderResults = DB.ResultsPoints.AsNoTracking().Include(rp => rp.RiderParticipation.Rider)
            .Where(rp => rp.StageId == stage.StageId).ToList()
            .Select(rp => (rp, GetStageSelectedEnum(rp.RiderParticipationId, stageSelection, teamSelection)));

        var stageResult = GetClassification(riderResults, "Stage", top5);
        var gcStandings = GetClassification(riderResults, "Gc", top5);
        var komStandings = GetClassification(riderResults, "Kom", top5);
        var pointsStandings = GetClassification(riderResults, "Points", top5);
        var youthStandings = GetClassification(riderResults, "Youth", top5);

        var response = new Classifications(gcStandings, pointsStandings, komStandings, youthStandings);

        return top5 ? response : response with { Stage = stageResult.ToList() };
    }

    private static StageSelectedEnum GetStageSelectedEnum(int riderParticipationId, List<int> stageSelection, List<int> teamSelection)
        => stageSelection.Contains(riderParticipationId)
            ? StageSelectedEnum.InStageSelection
            : teamSelection.Contains(riderParticipationId)
                ? StageSelectedEnum.InTeam
                : StageSelectedEnum.None;

    private static IEnumerable<ClassificationRow> GetClassification(IEnumerable<(ResultsPoint results, StageSelectedEnum selected)> resultsPoints, string field, bool top5)
        => resultsPoints
            .Where(rp => GetProperty(rp.results, field).Position > 0)
            .OrderBy(rp => GetProperty(rp.results, field).Position)
            .Select(rp => GetClassificationRow(rp.results, rp.selected, GetProperty(rp.results, field)))
            .Take(top5 ? 5 : int.MaxValue);

    private static BaseResult GetProperty(ResultsPoint rp, string field)
        => field switch
        {
            "Stage" => new BaseResult
            {
                Position = rp.StagePos,
                Score = rp.StageScore,
                Result = rp.StageResult
            },
            "Gc" => rp.Gc,
            "Points" => rp.Points,
            "Kom" => rp.Kom,
            "Youth" => rp.Youth,
            _ => throw new ArgumentException($"Invalid field: {field}")
        };

    private static ClassificationRow GetClassificationRow(ResultsPoint rp, StageSelectedEnum selected, BaseResult result)
        => new ClassificationRow
        {
            Rider = rp.RiderParticipation.Rider,
            Team = rp.RiderParticipation.Team,
            Result = result,
            Selected = selected
        };
}