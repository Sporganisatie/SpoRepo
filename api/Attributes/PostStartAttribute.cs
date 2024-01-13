using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PostStartAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var DB = context.HttpContext.RequestServices.GetService<DatabaseContext>();

        var raceIdString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "raceId").Value.FirstOrDefault();

        if (!DB.ShowResults(int.Parse(raceIdString), 1))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status423Locked);
            return;
        }
    }
}