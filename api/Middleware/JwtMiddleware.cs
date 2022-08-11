namespace SpoRE.Middleware;

// using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpoRE.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAccountService accountService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last(); // TODO zijn al deze stappen nodig?

        if (token != null)
            attachUserToContext(context, accountService, token);

        await _next(context);
    }

    private void attachUserToContext(HttpContext context, IAccountService accountService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("tijdelijke key");
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                // IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // attach user to context on successful jwt validation
            context.Items["User"] = accountService.GetById(accountId);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}