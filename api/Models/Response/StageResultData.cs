namespace SpoRE.Models.Response;

public record StageResultData(IEnumerable<UserScore> UserScores, IEnumerable<RiderScore> TeamResult, Classifications classifications, bool VirtualResult, bool FinalStandings);