using Ecoeden.User.Application.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog.Context;
using System.Diagnostics;

namespace User.Api.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RequestLoggerMiddleware(ILogger logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value);
            var url = context.Request.GetDisplayUrl();
            var verb = context.Request.Method;

            using (LogContext.PushProperty("Url", url))
            {
                LogContext.PushProperty("HttpMethod", verb);
                _logger.Here().Information("Http request starting");

                await _next(context);

                stopwatch.Stop();
                _logger.Here().Debug("Elapsed time {elapsedTime}", stopwatch.Elapsed);
                _logger.Here()
                        .Information("Http request completed. Response code {@code}", context.Response.StatusCode);
            }
        }
    }
}
