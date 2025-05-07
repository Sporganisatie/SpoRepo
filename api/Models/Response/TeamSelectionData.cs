using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record TeamSelectionData(
    int Budget,
    int BudgetOver,
    DateTime RaceStart,
    IEnumerable<RiderParticipation> Team,
    IEnumerable<SelectableRider> AllRiders,
    IEnumerable<string> AllTeams);
