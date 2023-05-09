using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record StageComparisonRider()
{
    public Rider Rider { get; internal set; }
    public int? StagePos { get; internal set; }
    public int TotalScore { get; internal set; }
    public SelectedEnum Selected { get; internal set; }
}