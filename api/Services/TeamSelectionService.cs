using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using Z.EntityFramework.Plus;

namespace SpoRE.Services;

public class TeamSelectionService(DatabaseContext DB, Userdata User)
{
    public TeamSelectionData GetTeamSelectionData(int raceId, bool budgetParticipation)
    {
        var budget = DB.RaceBudget(raceId, budgetParticipation);
        var maxRiderPrice = budgetParticipation ? 750_000 : int.MaxValue;

        var team = GetTeam().OrderBy(x => x.Type).ThenByDescending(x => x.Price).ThenBy(x => x.Rider.Lastname);
        var allRiders = AllRiders(raceId, maxRiderPrice).Select(rp => new SelectableRider(rp, Selectable(team, budget, rp)));
        var budgetOver = budget - team.Sum(x => x.Price);
        var allTeams = allRiders.Select(r => r.Details.Team).Distinct().Order();
        var starttime = DB.Stages
            .Where(s => s.RaceId == raceId && s.Stagenr == 1)
            .Select(s => s.Starttime)
            .FirstOrDefault() ?? DateTime.UtcNow.AddDays(10);

        starttime = DateTime.SpecifyKind(starttime, DateTimeKind.Utc);
        return new(budget, budgetOver, starttime, team, allRiders, allTeams);
    }

    public int AddRider(int riderParticipationId, int raceId, bool budgetParticipation)
    {
        var budget = DB.RaceBudget(raceId, budgetParticipation);
        var team = GetTeam();
        var toAdd = DB.RiderParticipations.Single(rp => rp.RiderParticipationId == riderParticipationId && rp.RaceId == raceId);

        if (Selectable(team, budget, toAdd) is SelectableEnum.Open)
        {
            DB.TeamSelections.Add(
                new()
                {
                    RiderParticipationId = riderParticipationId,
                    AccountParticipationId = User.ParticipationId
                });
            return DB.SaveChanges();
        }
        return 0; // TODO error?
    }

    private static SelectableEnum Selectable(IEnumerable<RiderParticipation> team, int budget, RiderParticipation toAdd)
    {
        if (team.Any(r => r.RiderParticipationId == toAdd.RiderParticipationId)) return SelectableEnum.Selected;

        if (team.Count() >= 20) return SelectableEnum.Max20;

        var budgetOver = budget - team.Sum(x => x.Price);
        var openSpaces = 20 - team.Count();
        var moneyNeededAfterAdding = (openSpaces - 1) * 500_000;
        var maxRiderPrice = budgetOver - moneyNeededAfterAdding;
        if (toAdd.Price > maxRiderPrice) return SelectableEnum.TooExpensive;

        var fourRiderteams = team.GroupBy(r => r.Team).Where(g => g.Count() >= 4).Select(g => g.Key);
        if (fourRiderteams.Contains(toAdd.Team)) return SelectableEnum.FourFromSameTeam;

        return SelectableEnum.Open;
    }

    internal int RemoveRider(int riderParticipationId)
    {
        var selectionRiders = DB.StageSelectionRiders
            .Where(sr => sr.StageSelection.AccountParticipationId == User.ParticipationId && sr.RiderParticipationId == riderParticipationId);
        DB.StageSelectionRiders.RemoveRange(selectionRiders);

        DB.TeamSelections.Remove(new() { RiderParticipationId = riderParticipationId, AccountParticipationId = User.ParticipationId });

        DB.StageSelections
            .Where(s => s.AccountParticipationId == User.ParticipationId && s.KopmanId == riderParticipationId)
            .Update(s => new StageSelectie { KopmanId = null });

        return DB.SaveChanges();
    }

    internal IEnumerable<RiderParticipation> GetTeam()
    {
        var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                    let ap = ts.AccountParticipation
                    where ap.AccountParticipationId == User.ParticipationId
                    orderby ts.RiderParticipation.Price descending
                    select ts.RiderParticipation;
        return query.ToList();
    }

    internal List<RiderParticipation> AllRiders(int raceId, int maxPrice)
        => DB.RiderParticipations
            .Include(rp => rp.Rider)
            .Where(rp => rp.RaceId == raceId && rp.Price <= maxPrice)
            .OrderByDescending(x => x.Price)
            .ToList();
}

public enum SelectableEnum // TODO move
{
    Open,
    TooExpensive,
    FourFromSameTeam,
    Max20,
    Selected
}
