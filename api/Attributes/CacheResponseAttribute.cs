using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace SpoRE.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CacheResponseAttribute : ActionFilterAttribute
{
    private IMemoryCache MemoryCache;

    public CacheResponseAttribute()
        => MemoryCache = null;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        MemoryCache ??= (IMemoryCache)context.HttpContext.RequestServices.GetService(typeof(IMemoryCache));

        if (MemoryCache.TryGetValue(CacheKey(context), out IActionResult cachedResult))
        {
            context.Result = cachedResult;
        }
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (!context.Canceled && !context.ExceptionHandled && MemoryCache != null)
        {
            var result = context.Result;
            if (result != null)
            {
                MemoryCache.Set(CacheKey(context), result, TimeSpan.FromSeconds(int.MaxValue));
            }
        }
    }

    private string CacheKey(ActionContext context)
        => $"{context.HttpContext.Request.Path + context.HttpContext.Request.QueryString}";
}
