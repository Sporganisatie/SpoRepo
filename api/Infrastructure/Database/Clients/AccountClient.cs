namespace SpoRE.Infrastructure.Database;

public class AccountClient
{
    DatabaseContext DatabaseContext;
    public AccountClient(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public Task<Result<Account>> Get(string email)
        => Result.For(DatabaseContext.Accounts.Single(x => x.Email.Equals(email))).AsTask(); // TODO hoe moet dit met Result, we willen errors afvangen zonder overal try/catch

    public Task<Result<Account>> Get(int id)
        => Result.For(DatabaseContext.Accounts.Single(x => x.AccountId == id)).AsTask();

    public Task<Result<AccountParticipation>> GetParticipation(int id, int raceId, bool budgetParticipation)
        => Result.For(
            DatabaseContext.AccountParticipations.Single(
                x => x.AccountId == id && x.RaceId == raceId && x.Budgetparticipation == budgetParticipation)).AsTask();

    public Task<Result<int>> GetParticipationCount(int id, int raceId)
        => Result.For(
            DatabaseContext.AccountParticipations.Count(
                x => x.AccountId == id && x.RaceId == raceId)).AsTask();
}