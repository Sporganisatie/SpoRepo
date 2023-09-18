using Microsoft.AspNetCore.Mvc.Filters;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ParticipationEndpointAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var DB = context.HttpContext.RequestServices.GetService<DatabaseContext>();
        var userData = context.HttpContext.RequestServices.GetService<Userdata>();

        var raceIdString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "raceId").Value.FirstOrDefault();
        var budgetParticipationString = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "budgetParticipation").Value.FirstOrDefault();

        if (int.TryParse(raceIdString, out var raceId) && raceId != 0 && bool.TryParse(budgetParticipationString, out var budgetParticipation))
        {
            var participation = DB.AccountParticipations.Single(
               x => x.AccountId == userData.Id && x.RaceId == raceId && x.BudgetParticipation == budgetParticipation);
            context.HttpContext.Items["participationId"] = participation.AccountParticipationId;
        };
    }
}