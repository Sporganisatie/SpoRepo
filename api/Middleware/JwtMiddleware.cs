using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Models.Settings;
using SpoRE.Services;
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

    public async Task Invoke(HttpContext context, IAccountService accountService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();

        if (token != null)
            attachUserToContext(context, accountService, token);

        await _next(context);
    }

    private void attachUserToContext(HttpContext context, IAccountService accountService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // attach user to context on successful jwt validation
            context.Items["user"] = accountService.GetById(accountId);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}