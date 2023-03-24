using SpoRE.Infrastructure.Database;

namespace SpoRE.Helper;

public class Userdata
{
    private IHttpContextAccessor Accessor { get; }
    public Userdata(IHttpContextAccessor accessor)
        => Accessor = accessor;

    public int Id => ((Account)Accessor.HttpContext.Items["user"]).AccountId;
}