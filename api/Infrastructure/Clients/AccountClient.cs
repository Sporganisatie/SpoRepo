namespace SpoRE.Infrastructure.Database;

public class AccountClient
{
    DatabaseContext DatabaseContext;
    public AccountClient(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public Task<Result<Account>> Get(string email)
        => Result.For(DatabaseContext.Accounts.Single(x => x.Email.Equals(email))).AsTask(); // TODO hoe moet dit met Result

    public Task<Result<Account>> Get(int id)
        => Result.For(DatabaseContext.Accounts.Single(x => x.AccountId == id)).AsTask();
}