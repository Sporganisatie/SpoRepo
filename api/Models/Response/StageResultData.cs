using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record StageResultData(IEnumerable<UserScore> UserScores, IEnumerable<ResultsPoint> TeamResult);