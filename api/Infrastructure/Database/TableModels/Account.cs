namespace SpoRE.Infrastructure.Database;

public class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public bool? Admin { get; set; }

    public bool Verified { get; set; }

    public virtual ICollection<AccountParticipation> AccountParticipations { get; } = new List<AccountParticipation>();

    public virtual ICollection<AccountToken> AccountTokens { get; } = new List<AccountToken>();
}
