using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SpoRE.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, DatabaseContext DB)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!token.IsNullOrEmpty())
            AttachUserToContextAsync(context, DB, token);

        await _next(context);
    }

    private void AttachUserToContextAsync(HttpContext context, DatabaseContext DB, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters //TODO handle expired error niet triggeren bij login endpoint
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // attach user to context on successful jwt validation
            var user = DB.Accounts.Single(x => x.AccountId == accountId);
            context.Items["user"] = user;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}