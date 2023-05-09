using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record StageComparisonRider()
{
    public Rider Rider { get; internal set; }
    public bool Kopman { get; internal set; }
    public int? StagePos { get; internal set; }
    public int TotalScore { get; internal set; }
    public StageSelectedEnum Selected { get; internal set; }
}

public enum StageSelectedEnum
{
    None,
    InTeam,
    InStageSelection
}