namespace SpoRE.Infrastructure.Database;

public class AccountParticipationStats
{
    public int AccountParticipationId { get; set; }

    public int TotaalGemist { get; set; }

    public int Positie { get; set; }

    public virtual AccountParticipation AccountParticipation { get; set; }
}
