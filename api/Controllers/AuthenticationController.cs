using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Input.Authentication;
using SpoRE.Services;

namespace SpoRE.Controllers;

[ApiController]
[Route("[controller]")]// de url voor alles hier is /authentication
public class AuthenticationController : ControllerBase
{
    private IAccountService AccountService;
    public AuthenticationController(IAccountService accountService)
    {
        AccountService = accountService;
    }

    [HttpPost("login")]// postcall naar /authentication/login
    public IActionResult Login(LoginCredentials credentials) // stuurt meteen een error message als de input niet klopt
    {
        var response = AccountService.Authenticate(credentials);

        if (response == string.Empty) // TODO explicite error terugkrijgen
            return BadRequest(new { message = "Username or password is incorrect" }); // wss iets anders dan badrequest returnen

        return Ok(response);
    }
}
