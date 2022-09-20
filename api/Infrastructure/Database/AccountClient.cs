using SpoRE.Infrastructure.Base;
using SpoRE.Services;

namespace SpoRE.Infrastructure.Database;

public class AccountClient : SqlDatabaseClient // nu is er wel een risico dat we meerdere connecties open zetten, misschien aparte base class die SqlDBclient als attribute heeft
{
    public async Task<Result<Account>> Get(string email)
    {
        var query = "SELECT * FROM account WHERE email = @email";
        var parameters = new Dictionary<string, object>()
        {
            {"email", email}
        };
        return await GetSingle<Account>(query, parameters);
    }

    public async Task<Result<Account>> Get(int id)
    {
        var query = "SELECT * FROM account WHERE account_id = @id";
        var parameters = new Dictionary<string, object>();
        parameters.Add("id", id);
        return await GetSingle<Account>(query, parameters);
    }
}