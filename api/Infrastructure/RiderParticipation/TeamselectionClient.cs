using SpoRE.Infrastructure.Base;

namespace SpoRE.Infrastructure.Database.Teamselection;

public class TeamselectionClient
{
    SqlDatabaseAdapter DatabaseClient;
    public TeamselectionClient(SqlDatabaseAdapter databaseClient)
    {
        DatabaseClient = databaseClient;
    }

    public async Task<Result<List<RiderParticipationRider>>> Get(int raceId)
        => await DatabaseClient.Get<RiderParticipationRider>(
            @$"SELECT * FROM rider_participation 
            INNER JOIN rider USING(rider_id)
            WHERE race_id = @raceId",
            new List<QueryParameter>(){new(raceId)}
        );
}

public record RiderParticipationRider // TODO move naar eigen file/folder, dit is effectief ook een FE model, Kunnen we die TS models laten genereren?
{
    public int RiderParticipationId { get; set; }
    public Rider Rider { get; set; }
    public int RaceId { get; set; }
    public int Price { get; set; }
    public bool Dnf { get; set; }
    public string Team { get; set; }
    public int Punch { get; set; }
    public int Climb { get; set; }
    public int Tt { get; set; }
    public int Sprint { get; set; }
    public int Gc { get; set; }
}

public record Rider // TODO move naar eigen file/folder, dit is effectief ook een FE model, Kunnen we die TS models laten genereren?
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Initials { get; set; }
    public string Country { get; set; }
    public int RiderId { get; set; }
}