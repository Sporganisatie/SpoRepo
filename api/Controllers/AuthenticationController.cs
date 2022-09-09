using System.Net;
using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Input.Authentication;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountService AccountService;
    public AuthenticationController(IAccountService accountService)
    {
        AccountService = accountService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginCredentials credentials)
    {
        var output = await AccountService.AuthenticateAsync(credentials);
        return output.IsValid ? output.Value : new UnauthorizedResult();
    }
}