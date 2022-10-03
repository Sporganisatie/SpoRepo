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
        .ActAsync(sqlResults => BuildStageTeamResults(sqlResults));

    private Result<IEnumerable<TeamResultRow>> BuildStageTeamResults(List<Infrastructure.Database.Stage.TeamResultRow> sqlResults)
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

public class TeamResultRow
{
    public Rider Rider { get; set; }
    public int StagePosition { get; set; }
    public int StagePoints { get; set; }
}

public record Rider // TODO move naar eigen file/folder
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

    internal static Rider From(RowWithRider input)
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