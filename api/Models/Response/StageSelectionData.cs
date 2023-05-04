using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record StageSelectionData(IEnumerable<RiderParticipation> Team); // TODO add top 5 per classification, with selected riders marked
