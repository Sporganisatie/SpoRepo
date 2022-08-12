using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Authentication;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private IAccountService AccountService;
    public AuthenticationController(IAccountService accountService)
    {
        AccountService = accountService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginCredentials credentials)
    {
        var response = AccountService.Authenticate(credentials);

        if (response == string.Empty) // TODO explicite error terugkrijgen
            return BadRequest(new { message = "Username or password is incorrect" }); // wss iets anders dan badrequest returnen

        return Ok(response);
    }
}
