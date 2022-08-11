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

        if (response == null) // TODO deze logica naar de service toe
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }
}
