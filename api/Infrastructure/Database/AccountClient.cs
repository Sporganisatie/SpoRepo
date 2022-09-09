using SpoRE.Infrastructure.Base;
using SpoRE.Services;

namespace SpoRE.Infrastructure.Database;

public static class AccountClient
{
    public static async Task<Result<Account>> Get(string email)
    {
        var query = "SELECT * FROM account WHERE email = @email";
        var parameters = new Dictionary<string, object>()
        {
            {"email", email}
        };
        return await SqlDatabaseClient.GetSingle<Account>(query, parameters);
    }

    public static async Task<Result<Account>> Get(int id)
    {
        var query = "SELECT * FROM account WHERE account_id = @id";
        var parameters = new Dictionary<string, object>();
        parameters.Add("id", id);
        return await SqlDatabaseClient.GetSingle<Account>(query, parameters);
    }
}