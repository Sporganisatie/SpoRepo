namespace SpoRE.Infrastructure.Database.Account;

public class Account // TODO deze wss aanpassen
{
    public int account_id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public bool admin { get; set; }
    public bool verified { get; set; }
    public Account() // TODO wss overbodige constructor
    {

    }
}