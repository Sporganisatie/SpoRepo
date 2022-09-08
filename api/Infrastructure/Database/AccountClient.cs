using SpoRE.Infrastructure.Base;
using SpoRE.Services;

namespace SpoRE.Infrastructure.Database;

public static class AccountClient
{
    public static async Task<Account> Get(string email)
    {
        var query = $"SELECT * FROM account WHERE email = '{email}'";
        return await SqlDatabaseClient.GetSingle<Account>(query);
    }

    public static async Task<Account> GetAsync(int id)
    {
        var query = $"SELECT * FROM account WHERE account_id = {id}";
        return await SqlDatabaseClient.GetSingle<Account>(query);
    }

}