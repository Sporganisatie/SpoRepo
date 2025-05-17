using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public partial class StatisticsService(DatabaseContext DB)
{
    public record UitvallersData(string UserName, int Uitvallers, int UitvallerBudget);

    public IEnumerable<UitvallersData> Uitvallers(int raceId, bool budgetParticipation)
    {
        var query = DB.AccountParticipations.Include(ap => ap.Account).Include(ap => ap.RiderParticipations).AsNoTracking()
            .Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation)
            .Select(ap => new UitvallersData(
                ap.Account.Username,
                ap.RiderParticipations.Count(rp => rp.Dnf),
                ap.RiderParticipations.Where(rp => rp.Dnf).Sum(rp => rp.Price)));

        return query.ToList().OrderByDescending(x => x.Uitvallers).ThenByDescending(x => x.UitvallerBudget);
    }

    public List<List<KlassementData>> Klassementen(int raceId, bool budgetParticipation)
    {
        var mostRecentFinished = DB.Stages.OrderByDescending(s => s.Stagenr).FirstOrDefault(s => s.Finished && s.RaceId == raceId);
        var baseQuery = DB.ResultsPoints
            .Include(rp => rp.RiderParticipation).ThenInclude(rp => rp.Rider)
            .Include(rp => rp.RiderParticipation).ThenInclude(rp => rp.AccountParticipations)
            .AsNoTracking()
            .Where(rp => rp.StageId == mostRecentFinished.StageId)
            .Select(rp => new BaseQueryResult(
                rp.RiderParticipation.Rider,
                rp.Gc,
                rp.Points,
                rp.Kom,
                rp.Youth,
                rp.RiderParticipation.AccountParticipations.Where(ap => ap.BudgetParticipation == budgetParticipation).Select(x => x.Account.Username))
            ).ToList();

        var gcQuery = GetKlassement(baseQuery, rp => rp.Gc);
        var pointsQuery = GetKlassement(baseQuery, rp => rp.Points);
        var komQuery = GetKlassement(baseQuery, rp => rp.Kom);
        var youthQuery = GetKlassement(baseQuery, rp => rp.Youth);

        return [gcQuery, pointsQuery, komQuery, youthQuery];
    }

    private static List<KlassementData> GetKlassement(List<BaseQueryResult> baseQuery, Func<BaseQueryResult, BaseResult> selector)
    {
        return baseQuery.Where(rp => selector(rp) is { Position: > 0 })
            .Select(rp => new KlassementData(selector(rp).Position.Value, selector(rp).Result, rp.Rider, rp.Accounts))
            .OrderBy(rp => rp.Position).Take(20).ToList();
    }

    private record BaseQueryResult(
        Rider Rider,
        BaseResult Gc,
        BaseResult Points,
        BaseResult Kom,
        BaseResult Youth,
        IEnumerable<string> Accounts);
}

public record KlassementData(
    int Position,
    string Result,
    Rider Rider,
    IEnumerable<string> Accounts);