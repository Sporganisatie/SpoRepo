
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Input.Authentication;
using SpoRE.Models.Settings;
using SpoRE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpoRE.Services;

public class Account // TODO deze verplaatsen en wss aanpassen
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
    public Task<Result<string>> AuthenticateAsync(LoginCredentials model);
    Task<Result<Account>> GetById(int id);
}

public class AccountService : IAccountService
{
    private readonly AppSettings _appSettings;

    public AccountService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public Task<Result<string>> AuthenticateAsync(LoginCredentials credentials)
        => AccountClient.Get(credentials.Email)
            .ActAsync(account => GenerateTokenForValidLogin(account, credentials));

    private Result<string> GenerateTokenForValidLogin(Account account, LoginCredentials credentials)
        => BCrypt.Net.BCrypt.Verify(credentials.Password, account.password)
            ? generateJwtToken(account)
            : Result.WithMessages<string>(new Error("Username or password is incorrect"));

    public Task<Result<Account>> GetById(int id)
        => AccountClient.Get(id);

    private string generateJwtToken(Account account)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", account.account_id.ToString()), new Claim("admin", account.admin.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //TODO meer over lezen
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}