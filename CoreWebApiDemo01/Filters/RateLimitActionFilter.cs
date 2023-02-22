using Lombok.NET;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace CoreWebApiDemo01.Filters;

public class RateLimitActionFilter: IAsyncActionFilter
{
    private readonly IMemoryCache _cache;

    public RateLimitActionFilter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        string ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
        string cacheKey = $"lastVisitTick_{ip}";
        long? lastTick = _cache.Get<long?>(cacheKey);

        if (lastTick == null || (Environment.TickCount64 - lastTick) > 1000)
        {
            _cache.Set(cacheKey, Environment.TickCount64, TimeSpan.FromSeconds(10));
            return next();
        }

        context.Result = new ContentResult() { StatusCode = 429 };
        return Task.CompletedTask;
    }
}