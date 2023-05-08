using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Database;

public class RaceClient
{
    DatabaseContext DB;
    public RaceClient(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public bool StageStarted(int raceId, int stagenr)
        => DateTime.UtcNow >= DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr).Starttime;

    public int CurrentStagenr(int raceId)
        => DB.Stages.Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).First()?.Stagenr ?? DB.Stages.Count(s => s.RaceId == raceId);

    public Stage CurrentStage(int raceId)
        => DB.Stages.Include(s => s.Race).Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).First();
}