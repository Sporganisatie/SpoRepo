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
    private readonly AccountClient _accountClient;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, AccountClient accountClient)
    {
        _next = next;
        _appSettings = appSettings.Value;
        _accountClient = accountClient;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!token.IsNullOrEmpty())
            await AttachUserToContextAsync(context, token);

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters //TODO handle expired error niet triggeren
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // attach user to context on successful jwt validation
            await _accountClient.Get(accountId)
                .ActAsync(user =>
                {
                    context.Items["user"] = user;
                    return Result.OK;
                });
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}