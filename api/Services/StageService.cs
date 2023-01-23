using SpoRE.Infrastructure.Database.Stage;

namespace SpoRE.Services;

public class StageService
{
    private StageClient _stageClient;

    public StageService(StageClient stageClient)
    {
        _stageClient = stageClient;
    }

    public Task<Result<IEnumerable<TeamResultRow>>> TeamResults(int raceId, int stagenr, bool budgetParticipation)
        => _stageClient.TeamResults(raceId, stagenr, budgetParticipation)
        .ActAsync(sqlResults => BuildTeamResults(sqlResults));

    private Result<IEnumerable<TeamResultRow>> BuildTeamResults(List<Infrastructure.Database.Stage.TeamResultRow> sqlResults) // TODO alias voor same name class
        => Result.For(sqlResults.Select(row =>
        {
            return new TeamResultRow()
            {
                Rider = Rider.From(row),
                StagePosition = row.stagepos,
                StagePoints = row.stagescore
            };
        }));
}

public class TeamResults
{
    public Kopman Kopman;
    public IEnumerable<TeamResultRow> SelectedRiders;
    public TeamResultsTotals Totals;
}

public class TeamResultsTotals
{
    public int StagePoints;
    public int GCPoints; // TODO GC of Gc
    public int PointsPoints; // TODO betere naam misschien NL?
    public int KomPoints; //TODO Kom of KOM
    public int YouthPoints;
    public int Total => StagePoints + GCPoints + PointsPoints + KomPoints + YouthPoints;

    public TeamResultsTotals(IEnumerable<Infrastructure.Database.Stage.TeamResultRow> selectedRiders, Kopman kopman) // TODO alias voor same name class
    {
        StagePoints = selectedRiders.Select(x => x.isKopman ? (int)(x.stagescore * 1.5) : x.stagescore).Sum() + kopman.Points;
        // TODO the rest when in db client model
        // GCPoints = selectedRiders.Select(x => x.gcscore).Sum();

    }
}

public class Kopman
{
    public Rider Rider { get; set; }
    public int Position { get; set; }
    public int Points { get; set; }
}

public class TeamResultRow
{
    public Rider Rider { get; set; }
    public int StagePosition { get; set; }
    public int StagePoints { get; set; }
}

public record Rider // TODO move naar eigen file/folder, dit is effectief ook een FE model, Kunnen we die TS models laten genereren?
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Initials { get; set; }
    public string Country { get; set; }
    public int RiderId { get; set; }
    // TODO extra fields bv fullname = first + last? 
    // initials + last
    // ingekort
    // (wss iets voor FE)
    public Rider()
    {

    }

    internal static Rider From(RiderRow input)
    {
        return new()
        {
            FirstName = input.firstname,
            LastName = input.lastname,
            Initials = input.initials,
            Country = input.country,
            RiderId = input.rider_id
        };
    }
}