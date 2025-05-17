using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public UniekheidResponse Uniekheid(int raceId, bool budgetParticipation)
    {
        decimal participants = DB.AccountParticipations.Count(x => x.RaceId == raceId && x.BudgetParticipation == budgetParticipation);

        var budget = DB.RaceBudget(raceId, budgetParticipation) / 100;

        var counts = DB.RiderParticipations.AsNoTrackingWithIdentityResolution()
                  .Where(rp => rp.AccountParticipations.Any(ap => ap.BudgetParticipation == budgetParticipation))
                  .GroupBy(rp => rp.RiderParticipationId)
                  .Select(grp => new
                  {
                      rpId = grp.Key,
                      Uniekheid = (participants - grp.Count()) * grp.First().Price / budget / (participants - 1),
                      grp.First().Dnf
                  });

        var renners = DB.RiderParticipations.Include(rp => rp.Rider).AsNoTracking()
            .Where(rp => rp.AccountParticipations.Any(ap => ap.BudgetParticipation == budgetParticipation) && rp.RaceId == raceId)
            .Select(rp => new UniekheidRennerRow(
                rp, //TODO afronding error
                Math.Round((participants - rp.AccountParticipations.Count(ap => ap.BudgetParticipation == budgetParticipation)) * rp.Price / budget / (participants - 1), 1),
                rp.AccountParticipations.Select(x => x.Account.Username))).ToList();

        var uniekheden = DB.AccountParticipations.Include(ap => ap.Account).Include(ap => ap.RiderParticipations).AsNoTracking()
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .ToList()
            .Select(ap => new
            {
                User = ap.Account.Username,
                Renners = renners.Where(x => ap.RiderParticipations.Contains(x.RiderParticipation)).ToList(),
            });

        var start = uniekheden.Select(x => new UniekheidRow(x.User, x.Renners.Sum(r => r.Uniekheid))).OrderByDescending(x => x.Uniekheid).ToList();
        var huidig = uniekheden.Select(x => new UniekheidRow(x.User, x.Renners.Where(r => !r.RiderParticipation.Dnf).Sum(r => r.Uniekheid))).OrderByDescending(x => x.Uniekheid).ToList();

        return new(start, huidig, renners.OrderByDescending(x => x.Uniekheid).ToList());
    }

    public OverlapResponse Overlap(int raceId, bool budgetParticipation)
    {
        var teamSelections = DB.AccountParticipations.Include(ap => ap.Account).Include(ap => ap.RiderParticipations).AsNoTracking()
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation).OrderBy(ap => ap.AccountId).ToList();

        var budget = DB.RaceBudget(raceId, budgetParticipation) / 100;

        var overlap = teamSelections.Select(ap => GetOverlap(ap, teamSelections, budget: false, budget));
        var overlapBudget = teamSelections.Select(ap => GetOverlap(ap, teamSelections, budget: true, budget));

        return new(overlap, overlapBudget);
    }

    private static OverlapRow GetOverlap(AccountParticipation ap, List<AccountParticipation> teamSelections, bool budget, int budgetAmount)
        => new(ap.Account.Username, teamSelections.ToDictionary(x => x.Account.Username, x => CalculateOverlap(x, ap, budget, budgetAmount)));

    private static int CalculateOverlap(AccountParticipation other, AccountParticipation main, bool budget, int budgetAmount)
    {
        if (main.Account.Username == other.Account.Username) return -1;
        var overlapRiders = main.RiderParticipations.Intersect(other.RiderParticipations, new RiderParticipationIdComparer());
        return budget
            ? overlapRiders.Sum(x => x.Price) / budgetAmount
            : overlapRiders.Count();
    }

    private class RiderParticipationIdComparer : IEqualityComparer<RiderParticipation>
    {
        public bool Equals(RiderParticipation x, RiderParticipation y)
        {
            if (x is null || y is null) return false;
            return x.RiderParticipationId == y.RiderParticipationId;
        }

        public int GetHashCode(RiderParticipation obj)
        {
            return obj.RiderParticipationId.GetHashCode();
        }
    }
}

public record UniekheidRow(string User, decimal Uniekheid);

public record UniekheidRennerRow(RiderParticipation RiderParticipation, decimal Uniekheid, IEnumerable<string> Accounts);

public record UniekheidResponse(IEnumerable<UniekheidRow> Start, IEnumerable<UniekheidRow> Huidig, IEnumerable<UniekheidRennerRow> Renners);

public record OverlapRow(string User, Dictionary<string, int> Overlaps);

public record OverlapResponse(IEnumerable<OverlapRow> Overlap, IEnumerable<OverlapRow> OverlapBudget);