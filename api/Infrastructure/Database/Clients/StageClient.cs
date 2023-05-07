using Microsoft.EntityFrameworkCore;

namespace SpoRE.Infrastructure.Database;

public class StageClient
{
    private readonly DatabaseContext DB;
    public StageClient(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    internal Stage MostRecentStartedStage()
        => DB.Stages.Include(s => s.Race).OrderByDescending(s => s.Starttime).ToList().First(s => s.Starttime < DateTime.UtcNow);
}
