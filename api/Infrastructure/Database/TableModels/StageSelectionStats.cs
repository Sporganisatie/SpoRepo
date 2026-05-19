namespace SpoRE.Infrastructure.Database;

public class StageSelectionStats
{
    public int StageSelectionId { get; set; }

    public int Optimaal { get; set; }

    public int Gemist { get; set; }

    public int DnfCount { get; set; }

    public int DnfBudget { get; set; }

    public int EtappePositie { get; set; }

    public bool EtappeLaatste { get; set; }

    public int StandPositie { get; set; }

    public bool StandLaatste { get; set; }

    public int StandChange { get; set; }

    public virtual StageSelectie StageSelection { get; set; }
}
