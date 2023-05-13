namespace SpoRE.Infrastructure.Database;

public class AccountParticipation
{
    public int AccountParticipationId { get; set; }

    public int AccountId { get; set; }

    public int RaceId { get; set; }

    public bool Budgetparticipation { get; set; }

    public int? Finalscore { get; set; }

    public virtual Account Account { get; set; }

    public virtual Race Race { get; set; }

    // public virtual ICollection<StageSelection> StageSelections { get; } = new List<StageSelection>();

    // public virtual ICollection<TeamSelection> TeamSelections { get; } = new List<TeamSelection>();
}
