using SpoRE.Infrastructure.Base;

namespace SpoRE.Infrastructure.Database.Account;

public class AccountClient
{
    SqlDatabaseClient DatabaseClient;
    public AccountClient(SqlDatabaseClient databaseClient)
    {
        DatabaseClient = databaseClient;
    }

    public async Task<Result<Account>> Get(string email)
    {
        var query = "SELECT * FROM account WHERE email = @email";
        var parameters = new Dictionary<string, object>() //TODO iets moois maken voor parameters misschien iets automatisch kwa naam?
        {
            {"email", email}
        };
        return await DatabaseClient.GetSingle<Account>(query, parameters);
    }

    public async Task<Result<Account>> Get(int id)
    {
        var query = "SELECT * FROM account WHERE account_id = @id";
        var parameters = new Dictionary<string, object>();
        parameters.Add("id", id);
        return await DatabaseClient.GetSingle<Account>(query, parameters);
    }
}