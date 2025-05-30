using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public record MissedPointsData(string Etappe, int Behaald, int Optimaal, int Gemist);

public record MissedPointsTable(string Username, List<MissedPointsData> Data);

public record GroupedData(int Stagenr, List<PointsData> Points);

public record PointsData(int Id, int? Stage, int Total);

public partial class StatisticsService
{
    internal IEnumerable<MissedPointsTable> MissedPoints(int raceId, bool budgetParticipation)
        => DB.AccountParticipations.Include(ss => ss.Account).Include(ap => ap.RiderParticipations).AsNoTracking()
            .Where(ss => ss.RaceId == raceId && ss.BudgetParticipation == budgetParticipation).ToList()
            .Select(MissedPointsUser).ToList();

    public MissedPointsTable MissedPointsUser(AccountParticipation user)
    {
        var ridersResults = DB.ResultsPoints
            .Join(DB.Stages, rp => rp.StageId, s => s.StageId, (rp, s) => new { Result = rp, Stage = s })
            .Where(joinedData => user.RiderParticipations.Select(rp => rp.RiderParticipationId).Contains(joinedData.Result.RiderParticipationId))
            .GroupBy(joinedData => joinedData.Stage.Stagenr)
            .Select(groupedData => new
            {
                Stagenr = groupedData.Key,
                Type = groupedData.First().Stage.Type,
                Points = groupedData.Select(g => new
                {
                    Id = g.Result.RiderParticipationId,
                    Stage = g.Result.StageScore,
                    Total = (int)(user.BudgetParticipation ? (g.Result.Stage.Type == StageType.TTT ? g.Result.Totalscore - g.Result.Teamscore - g.Result.StageScore : g.Result.Totalscore - g.Result.Teamscore) : g.Result.Totalscore)
                })
                .OrderByDescending(g => g.Total)
                .ToList()
            })
            .ToList();

        var actualScores = DB.StageSelections.Include(ss => ss.Stage).Where(ss => ss.AccountParticipationId == user.AccountParticipationId)
                .ToList().Where(ss => ss.Stage.Starttime < DateTime.UtcNow);

        var missedPoints = new List<MissedPointsData>();
        foreach (var riders in ridersResults)
        {
            var actualScore = actualScores.Single(a => a.Stage.Stagenr == riders.Stagenr).StageScore ?? 0;
            var optimalKopmanPoints = OptimalKopmanPoints(riders.Points.Select(p => new PointsData(p.Id, p.Stage, p.Total)));
            var optimalPoints = riders.Type is StageType.FinalStandings
                ? riders.Points.Sum(r => r.Total)
                : riders.Points.Take(9).Sum(r => r.Total) + optimalKopmanPoints;

            missedPoints.Add(new(riders.Stagenr.ToString(), actualScore, optimalPoints, optimalPoints - actualScore));
        }
        missedPoints.Add(new("Totaal", missedPoints.Sum(x => x.Behaald), missedPoints.Sum(x => x.Optimaal), missedPoints.Sum(x => x.Gemist)));
        return new(user.Account.Username, missedPoints);
    }

    private static int OptimalKopmanPoints(IEnumerable<PointsData> points)
    {
        var topStage = points.OrderByDescending(p => p.Stage).FirstOrDefault();
        if (!points.Take(9).Any(p => p.Id == topStage.Id) && topStage.Stage > 0 && topStage.Total > 0) throw new Exception("kopman niet in top 9");
        return (int)(topStage.Stage * 0.5);
    }
}