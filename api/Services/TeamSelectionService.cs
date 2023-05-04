using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class TeamSelectionService
{
    private readonly TeamSelectionClient Client;

    public TeamSelectionService(TeamSelectionClient client)
        => Client = client;

    public Task<Result<TeamSelectionData>> GetTeamSelectionData(int raceId, bool budgetParticipation)
        => Client.GetRaceInfo(raceId)
            .ActAsync(raceData => BuildResponse(raceData, raceId, budgetParticipation));

    private Result<TeamSelectionData> BuildResponse(Race raceData, int raceId, bool budgetParticipation)
    {
        // TODO check if race started return specific error, misschien interceptor op controller

        var budget = budgetParticipation ? 11_250_000 : raceData.Budget;
        var maxRiderPrice = budgetParticipation ? 750_000 : int.MaxValue;

        var team = Client.GetTeam();
        var allRiders = Client.GetAll(raceId, maxRiderPrice).Select(rp => new SelectableRider(rp, Selectable(team, raceData, rp)));
        var budgetOver = budget - team.Sum(x => x.Price);

        return new TeamSelectionData(budget, budgetOver, team, allRiders);
    }

    public Task<int> AddRider(int riderParticipationId, int raceId, bool budgetParticipation)
    //     => Client.GetRaceInfo(raceId)
    //     .ActAsync(raceData => {
    // // TODO check if race started return specific error, misschien interceptor op controller
    {
        var raceData = new Race();
        var budget = budgetParticipation ? 11_250_000 : raceData.Budget;
        var team = Client.GetTeam();
        var toAdd = Client.GetRider(riderParticipationId, raceId);
        if (Selectable(team, raceData, toAdd) is SelectableEnum.Open)
        {
            return Client.AddRider(riderParticipationId);
        }
        return Task.FromResult(0); // TODO error?
    }

    private static SelectableEnum Selectable(IEnumerable<RiderParticipation> team, Race raceData, RiderParticipation toAdd)
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

    internal object RemoveRider(int riderParticipationId, int raceId, bool budgetParticipation)
    {
        var raceData = Client.GetRaceInfo(raceId);
        // TODO check if race started return specific error, misschien interceptor op controller

        return Client.RemoveRider(riderParticipationId);
    }
}

public enum SelectableEnum // TODO move
{
    Open,
    TooExpensive,
    FourFromSameTeam,
    Selected
}
