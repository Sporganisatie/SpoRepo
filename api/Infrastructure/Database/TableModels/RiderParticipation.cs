namespace SpoRE.Infrastructure.Database;

public class RiderParticipation
{
    public int RiderParticipationId { get; set; }

    public int RaceId { get; set; }

    public int RiderId { get; set; }

    public int? Price { get; set; }

    public bool? Dnf { get; set; }

    public string Team { get; set; }

    public int? Punch { get; set; }

    public int? Climb { get; set; }

    public int? Tt { get; set; }

    public int? Sprint { get; set; }

    public int? Gc { get; set; }

    public virtual Race Race { get; set; }

    // public virtual ICollection<ResultsPoint> ResultsPoints { get; } = new List<ResultsPoint>();

    public virtual Rider Rider { get; set; }

    // public virtual ICollection<StageSelection> StageSelections { get; } = new List<StageSelection>();

    // public virtual ICollection<TeamSelection> TeamSelections { get; } = new List<TeamSelection>();

    // public virtual ICollection<StageSelection> StageSelectionsNavigation { get; } = new List<StageSelection>();
}
