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
        => DB.Stages.Where(s => s.Finished).Include(s => s.Race).OrderByDescending(s => s.Starttime).First();
}
