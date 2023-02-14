namespace SpoRE.Infrastructure.Database;

public class AccountToken
{
    public int TokenId { get; set; }

    public int AccountId { get; set; }

    public string Type { get; set; }

    public DateTime Expiry { get; set; }

    public string Token { get; set; }

    public virtual Account Account { get; set; }
}
