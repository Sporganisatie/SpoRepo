namespace SpoRE.Infrastructure.Database;

public class Stage
{
    public int StageId { get; set; }

    public int RaceId { get; set; }

    public int Stagenr { get; set; }

    public DateTime? Starttime { get; set; }

    public bool Finished { get; set; }

    public bool Complete { get; set; }

    public StageType Type { get; set; }

    public virtual Race Race { get; set; }

    public bool IsFinalStandings => Type is StageType.FinalStandings;

    // public virtual ICollection<ResultsPoint> ResultsPoints { get; } = new List<ResultsPoint>();

    // public virtual ICollection<StageSelection> StageSelections { get; } = new List<StageSelection>();
}

public enum StageType
{
    ITT,
    TTT,
    REG,
    FinalStandings,
    CLA
}
