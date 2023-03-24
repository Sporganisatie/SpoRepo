using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;
using SpoRE.Models.Settings;

namespace SpoRE.Services;

public class TeamSelectionService
{
    private readonly TeamSelectionClient Client;

    public TeamSelectionService(TeamSelectionClient client)
    {
        Client = client;
    }

    public TeamSelectionData GetTeamSelectionData(int raceId, bool budgetParticipation)
    {
        // TODO if race started return specific error
        var budget = budgetParticipation ? 11_250_000 : Client.GetBudget(raceId);
        var maxRiderPrice = budgetParticipation ? 750_000 : int.MaxValue;

        var team = Client.GetTeam(raceId, budgetParticipation);
        var allRiders = Client.GetAll(raceId, maxRiderPrice); // TODO add selectable
        var budgetOver = budget - team.Sum(x => x.Price);

        return new(budget, budgetOver, team, allRiders);
    }
}
