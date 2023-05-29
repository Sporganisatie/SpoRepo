using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Database;

public class RaceClient
{
    DatabaseContext DB;
    public RaceClient(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public bool ShowResults(int raceId, int stagenr)
    {
        var stage = DB.Stages.Single(x => x.RaceId == raceId && x.Stagenr == stagenr);
        return stage.Type == "FinalStandings" || DateTime.UtcNow >= stage.Starttime;
    }

    public int CurrentStagenr(int raceId)
        => DB.Stages.Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).FirstOrDefault()?.Stagenr ?? DB.Stages.Count(s => s.RaceId == raceId);

    public Stage CurrentStage(int raceId)
        => DB.Stages.Include(s => s.Race).Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).FirstOrDefault();
}