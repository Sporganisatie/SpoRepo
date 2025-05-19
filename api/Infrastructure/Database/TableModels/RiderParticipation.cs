using System.Text.Json.Serialization;

namespace SpoRE.Infrastructure.Database;

public class RiderParticipation
{
    public int RiderParticipationId { get; set; }

    public int RaceId { get; set; }

    public int RiderId { get; set; }

    public int Price { get; set; }

    public bool Dnf { get; set; }

    public string Team { get; set; }

    public int Punch { get; set; }

    public int Climb { get; set; }

    public int Tt { get; set; }

    public int Sprint { get; set; }

    public int Gc { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RiderType Type { get; set; }

    public virtual Race Race { get; set; }

    public virtual ICollection<ResultsPoint> ResultsPoints { get; } = [];

    public virtual Rider Rider { get; set; }

    public virtual ICollection<AccountParticipation> AccountParticipations { get; } = [];

    public override bool Equals(object obj)
    {
        if (obj is RiderParticipation other)
        {
            return RiderParticipationId == other.RiderParticipationId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return RiderParticipationId.GetHashCode();
    }
}

public enum RiderType
{
    Klassement,
    Klimmer,
    Sprinter,
    Tijdrijder,
    Aanvaller,
    Knecht
}
