using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class PreStartAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var raceClient = context.HttpContext.RequestServices.GetService<RaceClient>();

        var raceIdString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "raceId").Value.FirstOrDefault();
        var stagenrString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "stagenr").Value.FirstOrDefault();
        var stagenr = int.TryParse(stagenrString, out var stage) ? stage : 1;

        if (raceClient.StageStarted(int.Parse(raceIdString), stagenr))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status423Locked);
            return;
        }
    }
}