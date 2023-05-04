namespace SpoRE.Infrastructure.Database;

public class AccountClient
{
    DatabaseContext DatabaseContext;
    public AccountClient(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public Task<Result<Account>> Get(string email)
        => DatabaseContext.Accounts.SingleResult(x => x.Email.Equals(email)); // TODO hoe moet dit met Result, we willen errors afvangen zonder overal try/catch

    public Task<Result<Account>> Get(int id)
        => DatabaseContext.Accounts.SingleResult(x => x.AccountId == id);

    public Task<Result<AccountParticipation>> GetParticipation(int id, int raceId, bool budgetParticipation)
        => DatabaseContext.AccountParticipations.SingleResult(
                x => x.AccountId == id && x.RaceId == raceId && x.Budgetparticipation == budgetParticipation);
}