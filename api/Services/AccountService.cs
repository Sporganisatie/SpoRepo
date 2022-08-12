namespace SpoRE.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Models.Authentication;
using SpoRE.Models.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public record Account(int Id, string Email, string Password, bool admin);

public interface IAccountService
{
    public string Authenticate(LoginCredentials model);
    Account GetById(int id);
}

public class AccountService : IAccountService
{
    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
    private List<Account> _accounts = new List<Account>
    {
        new(1, "rens@mail.com", "mypass", true)
    };

    private readonly AppSettings _appSettings;

    public AccountService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public string Authenticate(LoginCredentials model)
    {
        var user = _accounts.SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password);

        return user == null
            ? string.Empty // TODO return expliciete error als auth niet klopt
            : generateJwtToken(user);
    }

    public Account GetById(int id)
    {
        return _accounts.FirstOrDefault(x => x.Id == id);
    }

    private string generateJwtToken(Account user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new Claim("admin", user.admin.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //TODO meer over lezen
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}