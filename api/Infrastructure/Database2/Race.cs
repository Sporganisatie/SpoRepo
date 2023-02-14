namespace SpoRE.Infrastructure.Database;

public partial class Race
{
    public int RaceId { get; set; }

    public string Name { get; set; }

    public int Year { get; set; }

    public int? Budget { get; set; }

    public bool? Finished { get; set; }

    public virtual ICollection<AccountParticipation> AccountParticipations { get; } = new List<AccountParticipation>();

    public virtual ICollection<RiderParticipation> RiderParticipations { get; } = new List<RiderParticipation>();

    public virtual ICollection<Stage> Stages { get; } = new List<Stage>();
}
