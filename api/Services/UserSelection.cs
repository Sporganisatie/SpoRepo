namespace SpoRE.Models.Response;

public record UserSelection(string Username, IEnumerable<StageComparisonRider> Riders, IEnumerable<StageComparisonRider> Gemist);
