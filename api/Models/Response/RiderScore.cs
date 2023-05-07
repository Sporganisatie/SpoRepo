using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record RiderScore()
{
    public Rider Rider { get; internal set; }
    public int? StagePos { get; internal set; }
    public int StageScore { get; internal set; }
    public int ClassificationScore { get; internal set; }
    public int TeamScore { get; internal set; }
    public int TotalScore { get; internal set; }
}