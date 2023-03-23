namespace SpoRE.Infrastructure.Database;

public class RiderParticipationClean
{
    public int RiderParticipationId { get; set; }

    public int? Price { get; set; }

    public string Team { get; set; }

    public int? Punch { get; set; }

    public int? Climb { get; set; }

    public int? Tt { get; set; }

    public int? Sprint { get; set; }

    public int? Gc { get; set; }

    public virtual Rider Rider { get; set; }

}
