using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PostStartAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var raceClient = context.HttpContext.RequestServices.GetService<RaceClient>();

        var raceIdString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "raceId").Value.FirstOrDefault();

        if (!raceClient.ShowResults(int.Parse(raceIdString), 1))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status423Locked);
            return;
        }
    }
}