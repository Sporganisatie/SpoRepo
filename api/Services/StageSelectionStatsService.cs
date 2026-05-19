using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class StageSelectionStatsService(DatabaseContext DB)
{
    public async Task Calculate(int stageId)
    {
        var stage = await DB.Stages.AsNoTracking().SingleAsync(s => s.StageId == stageId);

        var stageSelections = await DB.StageSelections
            .Include(ss => ss.AccountParticipation)
            .ThenInclude(ap => ap.RiderParticipations)
            .Include(ss => ss.StageSelectionStats)
            .Where(ss => ss.StageId == stageId)
            .ToListAsync();

        var prevStageSelections = await DB.StageSelections
            .Include(ss => ss.StageSelectionStats)
            .Where(ss => ss.Stage.RaceId == stage.RaceId && ss.Stage.Stagenr == stage.Stagenr - 1)
            .ToListAsync();

        var results = await DB.ResultsPoints.Where(rp => rp.StageId == stageId).ToListAsync();
        var (budget, regular) = GetEtappeRankings(stageSelections, prevStageSelections);

        foreach (var stageSelection in stageSelections)
        {
            var rankings = stageSelection.AccountParticipation.BudgetParticipation ? budget : regular;
            AddOrUpdateStats(stageSelection, results, rankings, stage.Type);
        }

        await DB.SaveChangesAsync();
    }

    private static void AddOrUpdateStats(StageSelectie stageSelection, List<ResultsPoint> results, RankingData rankings, StageType stageType)
    {
        var stats = stageSelection.StageSelectionStats ?? new StageSelectionStats();

        stats.Optimaal = OptimalPoints(stageSelection, results, stageType);
        stats.Gemist = stats.Optimaal - stageSelection.StageScore;

        stats.DnfCount = stageSelection.AccountParticipation.RiderParticipations.Count(rp => rp.Dnf); // TODO gaat fout bij herberekening van oudere etappe
        stats.DnfBudget = stageSelection.AccountParticipation.RiderParticipations.Where(rp => rp.Dnf).Sum(rp => rp.Price); // TODO gaat fout bij herberekening van oudere etappe

        var etappeRank = rankings.EtappeRanking.Single(r => r.StageSelectionId == stageSelection.StageSelectionId).Rank;
        var etappeLaatste = etappeRank == rankings.EtappeRanking.Max(r => r.Rank);
        stats.EtappePositie = etappeRank;
        stats.EtappeLaatste = etappeLaatste;

        var standRank = rankings.StandRanking.Single(r => r.StageSelectionId == stageSelection.StageSelectionId).Rank;
        var standLaatste = standRank == rankings.StandRanking.Max(r => r.Rank);
        stats.StandPositie = standRank;
        stats.StandLaatste = standLaatste;

        stats.StandChange = rankings.StandChange.Single(r => r.StageSelectionId == stageSelection.StageSelectionId).Rank;

        stageSelection.StageSelectionStats = stats;
    }

    #region rankings
    private static (RankingData budget, RankingData regular) GetEtappeRankings(List<StageSelectie> stageSelections, List<StageSelectie> prevStageSelections)
    {
        var budget = GetRankings(stageSelections.Where(ss => ss.AccountParticipation.BudgetParticipation).ToList(), prevStageSelections);
        var regular = GetRankings(stageSelections.Where(ss => !ss.AccountParticipation.BudgetParticipation).ToList(), prevStageSelections);
        return (budget, regular);
    }

    private static RankingData GetRankings(List<StageSelectie> stageSelecties, List<StageSelectie> prevStageSelections)
    {
        var etappeRanking = GetEtappeRanking(stageSelecties);
        var standRanking = GetStandRankings(stageSelecties);
        var standChange = standRanking.Select(sr =>
        {
            var prevRank = prevStageSelections.SingleOrDefault(ss => ss.AccountParticipationId == sr.AccountParticipationId)?.StageSelectionStats?.StandPositie;
            var change = prevRank.HasValue ? prevRank.Value - sr.Rank : 0;
            return new Ranking(sr.StageSelectionId, sr.AccountParticipationId, change);
        }).ToList();

        return new RankingData(etappeRanking, standRanking, standChange);
    }

    private static List<Ranking> GetEtappeRanking(List<StageSelectie> stageSelecties)
    {
        var ordered = stageSelecties.OrderByDescending(ss => ss.StageScore).ToList();
        var rank = 0;
        var extraRankIncreaseAfterTie = 0;
        var ranking = new List<Ranking>();
        for (int i = 0; i < ordered.Count; i++)
        {
            var user = ordered[i];
            if (rank == 0 || user.StageScore < ordered[i - 1].StageScore)
            {
                rank += 1 + extraRankIncreaseAfterTie;
                extraRankIncreaseAfterTie = 0;
            }
            else
            {
                extraRankIncreaseAfterTie++;
            }
            ranking.Add(new Ranking(user.StageSelectionId, user.AccountParticipationId, rank));
        }

        return ranking;
    }

    private static List<Ranking> GetStandRankings(List<StageSelectie> stageSelecties)
    {
        var ordered = stageSelecties.OrderByDescending(ss => ss.TotalScore).ToList();
        var rank = 0;
        var extraRankIncreaseAfterTie = 0;
        var ranking = new List<Ranking>();
        for (int i = 0; i < ordered.Count; i++)
        {
            var user = ordered[i];
            if (rank == 0 || user.StageScore < ordered[i - 1].TotalScore)
            {
                rank += 1 + extraRankIncreaseAfterTie;
                extraRankIncreaseAfterTie = 0;
            }
            else
            {
                extraRankIncreaseAfterTie++;
            }
            ranking.Add(new Ranking(user.StageSelectionId, user.AccountParticipationId, rank));
        }

        return ranking;
    }
    #endregion

    #region MissedPoints
    private static int OptimalPoints(StageSelectie stageSelection, List<ResultsPoint> results, StageType stageType)
    {
        var riderPoints = GetRiderPoints(stageSelection.AccountParticipation.RiderParticipations, results, stageType, stageSelection.AccountParticipation.BudgetParticipation);

        var optimalKopmanPoints = OptimalKopmanPoints(riderPoints.Select(p => new PointsData(p.Id, p.Stage, p.Total)));

        return stageType is StageType.FinalStandings
            ? riderPoints.Sum(r => r.Total)
            : riderPoints.Take(9).Sum(r => r.Total) + optimalKopmanPoints;
    }

    private static List<PointsData> GetRiderPoints(ICollection<RiderParticipation> riderParticipations, List<ResultsPoint> results, StageType stageType, bool budgetParticipation)
    {
        return results
            .Where(rp => riderParticipations.Select(rp => rp.RiderParticipationId).Contains(rp.RiderParticipationId))
            .Select(rp => GetPointsData(rp, stageType, budgetParticipation))
            .OrderByDescending(p => p.Total)
            .ToList();
    }

    private static PointsData GetPointsData(ResultsPoint rp, StageType stageType, bool budgetParticipation)
    {
        if (!budgetParticipation)
        {
            return new PointsData(rp.RiderParticipationId, rp.StageScore, rp.Totalscore);
        }

        var stagePoints = stageType == StageType.TTT ? rp.StageScore / 2 : rp.StageScore;
        var totalPoints = rp.Totalscore - rp.Teamscore;

        if (stageType == StageType.TTT) totalPoints -= stagePoints;

        return new PointsData(rp.RiderParticipationId, stagePoints, totalPoints);
    }

    private static int OptimalKopmanPoints(IEnumerable<PointsData> points)
    {
        var topStage = points.OrderByDescending(p => p.Stage).FirstOrDefault();
        if (!points.Take(9).Any(p => p.Id == topStage.Id) && topStage.Stage > 0 && topStage.Total > 0) throw new Exception("kopman niet in top 9");
        return (int)(topStage.Stage * 0.5);
    }
    #endregion

    private record Ranking(int StageSelectionId, int AccountParticipationId, int Rank);

    private record RankingData(List<Ranking> EtappeRanking, List<Ranking> StandRanking, List<Ranking> StandChange);
}
