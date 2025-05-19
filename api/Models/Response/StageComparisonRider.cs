using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public class StageComparisonRider
{
    public Rider Rider { get; internal set; }
    public bool Kopman { get; internal set; }
    public int? StagePos { get; internal set; }
    public int TotalScore { get; internal set; }
    public StageSelectedEnum Selected { get; internal set; }
    public bool Dnf { get; internal set; }

    public override bool Equals(object obj)
    {
        return obj is StageComparisonRider other &&
               Rider.PcsId == other.Rider.PcsId;
    }

    public override int GetHashCode()
    {
        return Rider.PcsId.GetHashCode();
    }
}

public enum StageSelectedEnum
{
    None,
    InTeam,
    InStageSelection
}