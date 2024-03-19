using Serilog.Context;
using User.Api.Extensions;

namespace User.Api.Middlewares
{
    public class CorrelationHeaderEnricher : IMiddleware
    {
        private const string CorrelationIdLogPropertyName = "CorrelationId";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = GetOrGenerateCorrelationId(context);
            using (LogContext.PushProperty("ThreadId", Environment.CurrentManagedThreadId))
            {
                LogContext.PushProperty(CorrelationIdLogPropertyName, correlationId);
                context.Response.Headers.Add("CorrelationId", correlationId);
                await next(context);
            }
        }

        private string GetOrGenerateCorrelationId(HttpContext context) => context?.Request.GetRequestHeaderOrdefault("CorrelationId", $"GEN-{Guid.NewGuid().ToString()}");
    }
}
