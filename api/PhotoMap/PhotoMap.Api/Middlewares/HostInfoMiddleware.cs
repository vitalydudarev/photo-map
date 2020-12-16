using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PhotoMap.Api.Middlewares
{
    public class HostInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public HostInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, HostInfo hostInfo)
        {
            hostInfo.Host = context.Request.Host;
            hostInfo.Scheme = context.Request.Scheme;

            await _next(context);
        }
    }
}
