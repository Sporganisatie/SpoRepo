namespace SpoRE.Models.Response;

public record StageSelectionData(IEnumerable<StageSelectableRider> Team, DateTime? Deadline); // TODO add top 5 per classification, with selected riders marked
