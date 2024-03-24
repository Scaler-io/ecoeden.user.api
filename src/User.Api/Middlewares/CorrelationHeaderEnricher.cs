using Serilog.Context;
using User.Api.Extensions;

namespace User.Api.Middlewares
{
    public class CorrelationHeaderEnricher 
    {
        private readonly RequestDelegate _next;

        public CorrelationHeaderEnricher(RequestDelegate next)
        {
            _next = next;
        }

        private const string CorrelationIdLogPropertyName = "CorrelationId";

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrGenerateCorrelationId(context);
            using (LogContext.PushProperty("ThreadId", Environment.CurrentManagedThreadId))
            {
                LogContext.PushProperty(CorrelationIdLogPropertyName, correlationId);
                context.Request.Headers.Add("CorrelationId", correlationId);
                await _next(context);
            }
        }

        private string GetOrGenerateCorrelationId(HttpContext context) => context?.Request.GetRequestHeaderOrdefault("CorrelationId", $"GEN-{Guid.NewGuid().ToString()}");
    }
}
