using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Database;

public static class DatabaseExtensions
{
    public static bool ShowResults(this DatabaseContext DB, int raceId, int stagenr)
    {
        var stage = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        return stage.Type == StageType.FinalStandings || DateTime.UtcNow >= stage.Starttime;
    }

    public static Stage CurrentStage(this DatabaseContext DB)
        => DB.Stages.Include(s => s.Race).Where(s => s.RaceId != 99 && !s.Race.Finished && !s.Complete).OrderBy(s => s.Starttime).FirstOrDefault();

    public static Stage CurrentStage(this DatabaseContext DB, int raceId)
        => DB.Stages.Include(s => s.Race).Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).FirstOrDefault();

    internal static Stage Aankomende(this DatabaseContext DB)
        => DB.Stages.Include(s => s.Race).OrderBy(s => s.Starttime).ToList().First(s => !s.Complete);

    internal static Stage MostRecentStartedStage(this DatabaseContext DB)
        => DB.Stages.Include(s => s.Race).OrderByDescending(s => s.Starttime).ToList().First(s => s.Starttime < DateTime.UtcNow);

    internal static int RaceBudget(this DatabaseContext DB, int raceId, bool budgetParticipation)
       => budgetParticipation ? 11_250_000 : DB.Races.Single(r => r.RaceId == raceId).Budget;
}