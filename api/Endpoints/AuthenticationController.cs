using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Input.Authentication;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AccountService AccountService;
    public AuthenticationController(AccountService accountService)
    {
        AccountService = accountService;
    }

    [HttpPost("login")]
    public ActionResult<string> Login(LoginCredentials credentials)
    {
        var output = AccountService.AuthenticateAsync(credentials);
        return output.IsValid ? output.Value : new UnauthorizedResult();
    }
}