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
    public async Task<IActionResult> Login(LoginCredentials credentials)
    {
        var response = await AccountService.AuthenticateAsync(credentials);

        if (response == string.Empty) // TODO explicite error terugkrijgen
            return BadRequest(new { message = "Username or password is incorrect" }); // wss iets anders dan badrequest returnen

        return Ok(response);
    }
}
