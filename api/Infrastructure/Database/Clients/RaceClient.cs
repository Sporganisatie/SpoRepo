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
}