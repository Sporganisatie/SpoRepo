using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class TeamSelectionService
{
    private readonly TeamSelectionClient Client;

    public TeamSelectionService(TeamSelectionClient client)
        => Client = client;

    public TeamSelectionData GetTeamSelectionData(int raceId, bool budgetParticipation)
    {
        var raceData = Client.GetRaceInfo(raceId);
        // uncomment if a new race/test data is available
        // if (raceData.Finished) return null; // TODO return specific error

        var budget = budgetParticipation ? 11_250_000 : raceData.Budget;
        var maxRiderPrice = budgetParticipation ? 750_000 : int.MaxValue;

        var team = Client.GetTeam(raceId, budgetParticipation);
        var allRiders = Client.GetAll(raceId, maxRiderPrice); // TODO add selectable
        var budgetOver = budget - team.Sum(x => x.Price);

        return new(budget, budgetOver, team, allRiders);
    }

    public void AddRider(int riderParticipationId, int raceId, bool budgetParticipation) // raceId optioneel
    {
        var raceData = Client.GetRaceInfo(raceId);
        var budget = budgetParticipation ? 11_250_000 : raceData.Budget;
        var team = Client.GetTeam(raceId, budgetParticipation);
        var toAdd = Client.GetRider(riderParticipationId);
        if (Selectable(team, raceData, toAdd) is SelectableEnum.Open)
        {
            Client.AddRider(riderParticipationId, raceId, budgetParticipation);
        }
        // else return error?
    }

    private SelectableEnum Selectable(IEnumerable<RiderParticipation> team, Race raceData, RiderParticipation toAdd)
    {
        if (team.Any(r => r.RiderParticipationId == toAdd.RiderParticipationId)) return SelectableEnum.Selected;

        var budgetOver = raceData.Budget - team.Sum(x => x.Price);
        var openSpaces = 20 - team.Count();
        var maxRiderPrice = budgetOver - openSpaces * 500_000;
        if (toAdd.Price > maxRiderPrice) return SelectableEnum.TooExpensive;

        var fourRiderteams = team.GroupBy(r => r.Team).Where(g => g.Count() >= 4).Select(g => g.Key);
        if (fourRiderteams.Contains(toAdd.Team)) return SelectableEnum.FourFromSameTeam;

        return SelectableEnum.Open;
    }
}

public enum SelectableEnum
{
    Open,
    TooExpensive,
    FourFromSameTeam,
    Selected
}
