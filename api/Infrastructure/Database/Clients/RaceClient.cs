namespace SpoRE.Infrastructure.Database;

public class RaceClient
{
    DatabaseContext DatabaseContext;
    public RaceClient(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public bool StageStarted(int raceId, int stagenr)
        => DateTime.Now >= DatabaseContext.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr).Starttime;

    public Stage StageInfo(int raceId, int stagenr)
        => DatabaseContext.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);

    public int CurrentStage(int raceId) // afhankelijk van finished/complete maken
        => DatabaseContext.Stages.Where(s => s.RaceId == raceId).ToList().OrderByDescending(s => s.Starttime).First(s => s.Starttime < DateTime.UtcNow)?.Stagenr ?? 0;
}