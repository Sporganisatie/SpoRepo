using Microsoft.AspNetCore.Mvc;
using SpoRE.Models.Authentication;
using SpoRE.Attributes;

namespace SpoRE.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TestDataController : ControllerBase
{
    public TestDataController() { }

    [HttpGet]
    public IActionResult GetTestData(LoginCredentials credentials)
    {
        return Ok("deze data komt vanaf de server");
    }
}
