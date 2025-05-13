namespace SpoRE.Infrastructure.Database;

public class Rider
{
    public int RiderId { get; set; }

    public string PcsId { get; set; }

    public string Country { get; set; }

    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public string Initials { get; set; }

    public virtual ICollection<RiderParticipation> RiderParticipations { get; } = [];
}
