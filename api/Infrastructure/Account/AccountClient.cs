using SpoRE.Infrastructure.Base;

namespace SpoRE.Infrastructure.Database.Account;

public class AccountClient
{
    SqlDatabaseClient DatabaseClient;
    public AccountClient(SqlDatabaseClient databaseClient)
    {
        DatabaseClient = databaseClient;
    }

    public Task<Result<Account>> Get(string email)
        => DatabaseClient.GetSingle<Account>("SELECT * FROM account WHERE email = @email", email);

    public Task<Result<Account>> Get(int id)
        => DatabaseClient.GetSingle<Account>("SELECT * FROM account WHERE account_id = @id", id);
}