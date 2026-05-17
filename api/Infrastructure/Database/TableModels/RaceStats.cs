namespace SpoRE.Infrastructure.Database;

public class RaceStats
{
    public int RaceId { get; set; }

    public bool BudgetParticipation { get; set; }

    public string Payload { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
