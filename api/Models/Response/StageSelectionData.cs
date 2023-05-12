namespace SpoRE.Models.Response;

public record StageSelectionData(IEnumerable<StageSelectableRider> Team, DateTime? Deadline, Classifications Classifications);
