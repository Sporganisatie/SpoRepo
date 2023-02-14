
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Models.Input.Authentication;
using SpoRE.Models.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpoRE.Services;

public class AccountService
{
    private readonly AppSettings _appSettings;

    public AccountService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public Task<Result<string>> AuthenticateAsync(LoginCredentials credentials)
        => Result.For(new Account()).AsTask() // TODO actual Call
            .ActAsync(account => GenerateTokenForValidLogin(account, credentials));

    private Result<string> GenerateTokenForValidLogin(Account account, LoginCredentials credentials)
        => BCrypt.Net.BCrypt.Verify(credentials.Password, account.Password)
            ? generateJwtToken(account)
            : Result.WithMessages<string>(new Error("Username or password is incorrect"));

    private string generateJwtToken(Account account)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", account.AccountId.ToString()), new Claim("admin", account.Admin.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}