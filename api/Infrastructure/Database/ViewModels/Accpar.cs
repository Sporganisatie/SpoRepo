namespace SpoRE.Infrastructure.Database;

public partial class Accpar
{
    public int? AccountId { get; set; }
    public string Username { get; set; }
    public int? AccountParticipationId { get; set; }
    public bool? BudgetParticipation { get; set; }
    public int? RaceId { get; set; }
}
