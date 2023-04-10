using Microsoft.AspNetCore.Mvc.Filters;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ParticipationEndpointAttribute : Attribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }

    public async void OnActionExecuting(ActionExecutingContext context)
    {
        var accountClient = context.HttpContext.RequestServices.GetService<AccountClient>();
        var userData = context.HttpContext.RequestServices.GetService<Userdata>();

        var raceIdString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "raceId").Value.FirstOrDefault();
        var budgetParticipationString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "budgetParticipation").Value.FirstOrDefault();

        if (int.TryParse(raceIdString, out var raceId) && raceId != 0)
        {
            bool.TryParse(budgetParticipationString, out var budgetParticipation);
            await accountClient.GetParticipation(userData.Id, raceId, budgetParticipation)
                .ActAsync(participation =>
                {
                    context.HttpContext.Items["participationId"] = participation.AccountParticipationId;
                    return Result.OK;
                });
        };
        // else misschien meteen een error gooien?
    }
}