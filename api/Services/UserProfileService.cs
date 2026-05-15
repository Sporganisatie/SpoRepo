using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class UserProfileService(DatabaseContext DB, Userdata User)
{
    private const int SpacerScoreMarker = -1;

    public UserProfileData GetProfile(string username, bool budgetParticipation, int selectedRaceId)
    {
        var target = DB.Accounts.SingleOrDefault(a => a.Username == username);

        var viewer = DB.Accounts.Single(a => a.AccountId == User.Id);

        if (target is null)
        {
            return null;
        }

        var participations = DB.AccountParticipations
            .Include(ap => ap.Race)
            .Where(ap => ap.AccountId == target.AccountId && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        var overview = participations
            .Select(ap => ap.Race)
            .OrderByDescending(race => race.Year)
            .ThenByDescending(race => race.Name)
            .ToList();

        if (!participations.Any(p => p.RaceId == selectedRaceId))
        {
            return new UserProfileData(
                target.Username,
                viewer.Username,
                null,
                overview);
        }

        var results = DB.ResultsPoints.Where(rp => rp.Stage.RaceId == selectedRaceId).AsNoTrackingWithIdentityResolution().ToList();

        var viewerParticipationId = DB.AccountParticipations
            .SingleOrDefault(ap => ap.AccountId == viewer.AccountId && ap.RaceId == selectedRaceId && ap.BudgetParticipation == budgetParticipation)?.AccountParticipationId ?? 0;

        var targetTeam = GetTeam(results, participations.Single(ap => ap.RaceId == selectedRaceId).AccountParticipationId, budgetParticipation);
        var viewerTeam = GetTeam(results, viewerParticipationId, budgetParticipation);
        var currentRaceTeams = SplitTeams(targetTeam, viewerTeam);

        return new UserProfileData(
            target.Username,
            viewer.Username,
            currentRaceTeams,
            overview);
    }

    private static HuidigeRaceTeams SplitTeams(List<StageComparisonRider> targetTeam, List<StageComparisonRider> viewerTeam)
    {
        var viewerByPcsId = viewerTeam.ToDictionary(rider => rider.Rider.PcsId);
        var bothSelected = new List<CombinedSelectedRider>();
        var targetUnique = new List<StageComparisonRider>();

        foreach (var targetRider in targetTeam)
        {
            if (viewerByPcsId.TryGetValue(targetRider.Rider.PcsId, out var viewerRider))
            {
                bothSelected.Add(new CombinedSelectedRider(targetRider, viewerRider));
                viewerByPcsId.Remove(targetRider.Rider.PcsId);
            }
            else
            {
                targetUnique.Add(targetRider);
            }
        }

        var viewerUnique = viewerByPcsId.Values.ToList();

        // Separate DNF and non-DNF for bothSelected
        var bothSelectedNonDnf = bothSelected
            .Where(x => !x.TargetRider.Dnf && !x.ViewerRider.Dnf)
            .OrderByDescending(x => (x.TargetRider.TotalScore + x.ViewerRider.TotalScore) / 2.0)
            .ToList();

        var bothSelectedDnf = bothSelected
            .Where(x => x.TargetRider.Dnf || x.ViewerRider.Dnf)
            .OrderByDescending(x => (x.TargetRider.TotalScore + x.ViewerRider.TotalScore) / 2.0)
            .ToList();

        // Separate DNF and non-DNF for targetUnique
        var targetUniqueNonDnf = targetUnique
            .Where(x => !x.Dnf)
            .OrderByDescending(x => x.TotalScore)
            .ToList();

        var targetUniqueDnf = targetUnique
            .Where(x => x.Dnf)
            .OrderByDescending(x => x.TotalScore)
            .ToList();

        // Separate DNF and non-DNF for viewerUnique
        var viewerUniqueNonDnf = viewerUnique
            .Where(x => !x.Dnf)
            .OrderByDescending(x => x.TotalScore)
            .ToList();

        var viewerUniqueDnf = viewerUnique
            .Where(x => x.Dnf)
            .OrderByDescending(x => x.TotalScore)
            .ToList();

        // Add empty rows only to the shorter unique list, so both unique tables align.
        var uniqueDifference = Math.Abs(targetUniqueNonDnf.Count - viewerUniqueNonDnf.Count);
        if (targetUniqueNonDnf.Count < viewerUniqueNonDnf.Count)
        {
            targetUniqueNonDnf.AddRange(Enumerable.Range(0, uniqueDifference)
                .Select(_ => new StageComparisonRider { Rider = null, TotalScore = SpacerScoreMarker, Dnf = false }));
        }
        else if (viewerUniqueNonDnf.Count < targetUniqueNonDnf.Count)
        {
            viewerUniqueNonDnf.AddRange(Enumerable.Range(0, uniqueDifference)
                .Select(_ => new StageComparisonRider { Rider = null, TotalScore = SpacerScoreMarker, Dnf = false }));
        }

        // Add total rows with totalScore set to actual total and marker for identification
        var targetTotal = bothSelectedNonDnf.Sum(x => x.TargetRider.TotalScore);
        var viewerTotal = bothSelectedNonDnf.Sum(x => x.ViewerRider.TotalScore);
        bothSelectedNonDnf.Add(new CombinedSelectedRider(
            new StageComparisonRider { Rider = null, TotalScore = targetTotal, Dnf = false },
            new StageComparisonRider { Rider = null, TotalScore = viewerTotal, Dnf = false }
        ));

        var targetUniqueTotal = targetUniqueNonDnf
            .Where(x => x.TotalScore != SpacerScoreMarker)
            .Sum(x => x.TotalScore);
        targetUniqueNonDnf.Add(new StageComparisonRider { Rider = null, TotalScore = targetUniqueTotal, Dnf = false });

        var viewerUniqueTotal = viewerUniqueNonDnf
            .Where(x => x.TotalScore != SpacerScoreMarker)
            .Sum(x => x.TotalScore);
        viewerUniqueNonDnf.Add(new StageComparisonRider { Rider = null, TotalScore = viewerUniqueTotal, Dnf = false });

        return new HuidigeRaceTeams(bothSelectedNonDnf, bothSelectedDnf, targetUniqueNonDnf, targetUniqueDnf, viewerUniqueNonDnf, viewerUniqueDnf);
    }

    public List<StageComparisonRider> GetTeam(List<ResultsPoint> results, int accountParticipationId, bool budgetParticipation)
    {
        var stageSelections = DB.StageSelections.Include(ss => ss.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking().Where(ss => ss.AccountParticipationId == accountParticipationId);
        var riderParticipations = stageSelections.SelectMany(ss => ss.RiderParticipations.Select(rp => new
        {
            RiderParticipation = rp,
            IsKopman = ss.KopmanId == rp.RiderParticipationId,
            ss.StageId
        })).ToList();

        var riderScores = riderParticipations.Join(
            results,
            rp => new { rp.RiderParticipation.RiderParticipationId, rp.StageId },
            res => new { res.RiderParticipationId, res.StageId },
            (rp, res) => new { rp.RiderParticipation, TotalScore = (rp.IsKopman ? res.StageScore * 0.5 : 0) + res.Totalscore - (budgetParticipation ? res.Teamscore : 0) }
        ).GroupBy(rp => rp.RiderParticipation).Select(rp => new StageComparisonRider
        {
            Rider = rp.Key.Rider,
            TotalScore = (int)rp.Sum(x => x.TotalScore),
            Dnf = rp.Key.Dnf
        }).ToList().OrderByDescending(x => x.TotalScore);

        var nietOpgesteld = DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking()
            .Single(ap => ap.AccountParticipationId == accountParticipationId).RiderParticipations.Select(rp => new StageComparisonRider
            {
                Rider = rp.Rider,
                TotalScore = 0,
                Dnf = rp.Dnf
            }).Where(rp => !riderScores.Contains(rp));

        return riderScores.Union(nietOpgesteld).ToList();
    }
}

public record UserProfileData(
    string Username,
    string ViewerUsername,
    HuidigeRaceTeams HuidigeRaceTeams,
    List<Race> Overview
);

public record HuidigeRaceTeams(
    List<CombinedSelectedRider> BothSelected,
    List<CombinedSelectedRider> BothSelectedDnf,
    List<StageComparisonRider> TargetUnique,
    List<StageComparisonRider> TargetUniqueDnf,
    List<StageComparisonRider> ViewerUnique,
    List<StageComparisonRider> ViewerUniqueDnf
);

public record CombinedSelectedRider(StageComparisonRider TargetRider, StageComparisonRider ViewerRider);
