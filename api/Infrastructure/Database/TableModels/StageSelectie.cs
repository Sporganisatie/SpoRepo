namespace SpoRE.Infrastructure.Database;

public class StageSelectie
{
    public int StageSelectionId { get; set; }

    public int AccountParticipationId { get; set; }

    public int StageId { get; set; }

    public int StageScore { get; set; }

    public int TotalScore { get; set; }

    public int? KopmanId { get; set; }

    public virtual AccountParticipation AccountParticipation { get; set; }

    public virtual Stage Stage { get; set; }

    public virtual ICollection<RiderParticipation> RiderParticipations { get; } = [];
}
