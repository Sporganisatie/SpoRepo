using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;
using SpoRE.Infrastructure.Database.Account;

namespace SpoRE.Controllers;

[ApiController]
[Route("api/[controller]")]// de url voor alles hier is /testdata
[Authorize] // betekent dat alle calls eerst door authorizeattribute heengaan
public class TestDataController : ControllerBase //ControllerBase is nodig voor Ok(response) moeten we nog vervangen
{
    private readonly IHttpContextAccessor _httpContextAccessor; // deze hebben we nodig om bij de user te komen die op de context zit
    public TestDataController(IHttpContextAccessor httpContextAccessor) // wordt in program toegevoegd en kan je zo gewoon in de constructor gooien
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet] // gewoon get op /testdata
    public IActionResult GetTestData()
    {
        var user = (Account)_httpContextAccessor.HttpContext.Items["user"];
        return Ok("deze data komt vanaf de server user: " + user.username);
    }
}
