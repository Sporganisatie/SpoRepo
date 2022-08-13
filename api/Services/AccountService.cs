namespace SpoRE.Services;

using SpoRE.Infrastructure.SqlDatabaseClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Models.Input.Authentication;
using SpoRE.Models.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

public class Account
{
    public int account_id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public bool admin { get; set; }
    public bool verified { get; set; }
    public Account()
    {

    }
}

public interface IAccountService
{
    public string Authenticate(LoginCredentials model);
    Account GetById(int id);
}

public class AccountService : IAccountService
{
    private readonly AppSettings _appSettings;

    public AccountService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public string Authenticate(LoginCredentials credentials)
    {
        var user = SqlDatabaseClient.GetAccount(credentials.Email);

        if (user is null || !BCrypt.Verify(credentials.Password, user.password))
        {
            return string.Empty;
        }
        return generateJwtToken(user);
    }

    public Account GetById(int id)
    {
        return SqlDatabaseClient.GetAccount(id);
    }

    private string generateJwtToken(Account user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.account_id.ToString()), new Claim("admin", user.admin.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //TODO meer over lezen
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}