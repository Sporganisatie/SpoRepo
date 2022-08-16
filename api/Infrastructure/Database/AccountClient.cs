using SpoRE.Infrastructure.Base;
using SpoRE.Services;

namespace SpoRE.Infrastructure.Database;

public static class AccountClient
{
    public static Account Get(string email)
    {
        var query = $"SELECT * FROM account WHERE email = '{email}'";
        var output = SqlDatabaseClient.ProcessQuery<Account>(query);
        if (output.Item1.Count != 1)
        {
            return null;
        }
        return output.Item1.Single();
    }

    public static Account Get(int id)
    {
        var query = $"SELECT * FROM account WHERE account_id = {id}";
        var output = SqlDatabaseClient.ProcessQuery<Account>(query);
        if (output.Item1.Count != 1)
        {
            return null;
        }
        return output.Item1.Single();
    }

}