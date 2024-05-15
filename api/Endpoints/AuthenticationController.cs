using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Input.Authentication;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(AccountService Service) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<string> Login(LoginCredentials credentials)
    {
        var output = Service.AuthenticateAsync(credentials);
        return output.IsValid ? output.Value : new UnauthorizedResult();
    }
}