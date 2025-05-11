namespace SpoRE.Infrastructure.Database;

public static class DatabaseExtensions
{
    public static bool ShowResults(this DatabaseContext DB, int raceId, int stagenr)
    {
        var stage = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        return stage.Type == StageType.FinalStandings || DateTime.UtcNow >= stage.Starttime;
    }

    internal static int RaceBudget(this DatabaseContext DB, int raceId, bool budgetParticipation)
       => budgetParticipation ? 11_250_000 : DB.Races.Single(r => r.RaceId == raceId).Budget;
}