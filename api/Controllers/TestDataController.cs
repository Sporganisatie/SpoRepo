using Dotnet2.Infrastructure.SqlDatabaseClient;
using Microsoft.AspNetCore.Mvc;
using SpoRE.Attributes;

namespace SpoRE.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TestDataController : ControllerBase
{
    public TestDataController() { }

    [HttpGet]
    public IActionResult GetTestData()
    {
        return Ok("deze data komt vanaf de server");
    }

    [HttpGet("sql")]
    public IActionResult MakeDBcall(string query)
    {
        return Ok(SqlDatabaseClient.ProcessFreeQuery(query));
    }
}
