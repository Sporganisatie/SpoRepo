using SpoRE.Infrastructure.Base;

namespace SpoRE.Infrastructure.Database.Stage;
public class StageClient
{
    const string rider_fields = "firstname, lastname, initials, country, rider_id";
    SqlDatabaseClient DatabaseClient;
    public StageClient(SqlDatabaseClient databaseClient)
    {
        DatabaseClient = databaseClient;
    }

    public async Task<Result<List<TeamResultRow>>> TeamResults(int raceId, int stageNr, bool budgetParticipation)
    {
        var accountId = 2;

        var teamQuery = @$"SELECT {rider_fields}, stagepos, results_points.stagescore FROM stage_selection_rider
            INNER JOIN rider_participation USING(rider_participation_id)
            INNER JOIN rider USING(rider_id)
            INNER JOIN stage_selection USING(stage_selection_id)
            INNER JOIN stage USING(stage_id)
            INNER JOIN account_participation USING(account_participation_id)
            LEFT JOIN results_points ON results_points.rider_participation_id = rider_participation.rider_participation_id  AND results_points.stage_id = stage.stage_id
            WHERE account_id = @accountId AND budgetparticipation = @budgetParticipation AND stagenr = @stageNr AND stage.race_id = @raceId";

        var kopmanQuery = @$"SELECT {rider_fields}, stagepos, results_points.stagescore * 0.5 FROM stage_selection
            INNER JOIN stage USING(stage_id)
            INNER JOIN rider_participation ON stage_selection.kopman_id = rider_participation.rider_participation_id
            INNER JOIN rider USING(rider_id)
            INNER JOIN account_participation USING(account_participation_id)
            LEFT JOIN results_points ON results_points.rider_participation_id = rider_participation.rider_participation_id  AND results_points.stage_id = stage.stage_id
            WHERE account_id = @accountId AND budgetparticipation = @budgetParticipation AND stagenr = @stageNr AND stage.race_id = @raceId";

        var parameters = new List<QueryParameter>()
        {
            new(accountId),
            new(budgetParticipation),
            new(stageNr),
            new(raceId)
        };
        var batch = new List<Query>()
        {
            new(teamQuery, parameters),
            new(kopmanQuery, parameters)
        };
        return await DatabaseClient.Get<TeamResultRow>(batch);
    }
}

public class Query // TODO move
{
    public List<QueryParameter> Parameters { get; set; }
    public string QueryString { get; set; }

    public Query(string query, IEnumerable<QueryParameter> parameters) // TODO ff nadenken over list vs IEnumerable
    {
        QueryString = query;
        Parameters = (List<QueryParameter>)parameters;
    }

    public Query(string query)
    {
        QueryString = query;
        Parameters = new List<QueryParameter>();
    }
}

public record TeamResultRow : RowWithRider
{
    public TeamResultRow()
    { }
    public int stagepos { get; set; }
    public int stagescore { get; set; }
}

public record RowWithRider
{
    public string lastname { get; set; }
    public string firstname { get; set; }
    public string initials { get; set; }
    public string country { get; set; }
    public int rider_id { get; set; }
}