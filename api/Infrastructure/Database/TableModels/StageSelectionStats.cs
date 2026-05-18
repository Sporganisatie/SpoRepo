namespace SpoRE.Infrastructure.Database;

public class StageSelectionStats
{
    public int StageSelectionId { get; set; }

    public int Optimaal { get; set; }

    public int Gemist { get; set; }

    public int DnfCount { get; set; }

    public int DnfBudget { get; set; }

    public int Positie { get; set; } // TODO rename in DB als we ook klassement positie willen opslaan

    public bool Laatste { get; set; } // TODO rename in DB als we ook klassement positie willen opslaan

    // public int PositieNaEtappe { get; set; }

    // public int LaatsteNaEtappe { get; set; }

    public virtual StageSelectie StageSelection { get; set; }
}
