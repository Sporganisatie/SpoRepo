
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Infrastructure.Database.Account;
using SpoRE.Models.Input.Authentication;
using SpoRE.Models.Settings;
using SpoRE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpoRE.Services;

public class AccountService
{
    private readonly AppSettings _appSettings;
    private AccountClient _accountClient;

    public AccountService(IOptions<AppSettings> appSettings, AccountClient accountClient)
    {
        _appSettings = appSettings.Value;
        _accountClient = accountClient;
    }

    public Task<Result<string>> AuthenticateAsync(LoginCredentials credentials)
        => _accountClient.Get(credentials.Email)
            .ActAsync(account => GenerateTokenForValidLogin(account, credentials));

    private Result<string> GenerateTokenForValidLogin(Account account, LoginCredentials credentials)
        => BCrypt.Net.BCrypt.Verify(credentials.Password, account.password)
            ? generateJwtToken(account)
            : Result.WithMessages<string>(new Error("Username or password is incorrect"));

    private string generateJwtToken(Account account)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", account.account_id.ToString()), new Claim("admin", account.admin.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}