using SpoRE.Infrastructure.Database;

namespace SpoRE.Helper;

public class Userdata(IHttpContextAccessor Accessor)
{
    public int Id => ((Account)Accessor.HttpContext.Items["user"]).AccountId;
    public int ParticipationId => (int)Accessor.HttpContext.Items["participationId"];
}