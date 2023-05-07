namespace SpoRE.Infrastructure.Database;

public class StageSelection
{
    public int StageSelectionId { get; set; }

    public int AccountParticipationId { get; set; }

    public int StageId { get; set; }

    public int Stagescore { get; set; }

    public int Totalscore { get; set; }

    public int? KopmanId { get; set; }

    public virtual AccountParticipation AccountParticipation { get; set; }

    public virtual RiderParticipation Kopman { get; set; }

    public virtual Stage Stage { get; set; }
}
