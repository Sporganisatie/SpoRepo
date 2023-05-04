using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record StageSelectableRider(RiderParticipation Rider, bool Selected, bool IsKopman);