namespace SpoRE.Infrastructure.Database;

public class StageStats
{
    public int RaceId { get; set; }

    public int Stagenr { get; set; }

    public string Username { get; set; }

    public bool BudgetParticipation { get; set; }

    public int StageScore { get; set; }

    public int CumulativeScore { get; set; }

    public int MissedPoints { get; set; }

    public int DnfCount { get; set; }

    public int DnfBudget { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
