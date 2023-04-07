using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record TeamSelectionData(int Budget, int BudgetOver, IEnumerable<RiderParticipation> Team, IEnumerable<SelectableRider> AllRiders);
