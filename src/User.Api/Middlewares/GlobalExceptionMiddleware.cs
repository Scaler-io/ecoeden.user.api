using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Mime;

namespace User.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(ILogger logger, IWebHostEnvironment environment, RequestDelegate next)
        {
            _logger = logger;
            _environment = environment;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }catch (Exception ex)
            {
                await HandleGlobalException(context, ex);
            }
        }

        private async Task HandleGlobalException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = _environment.IsDevelopment()
                            ? new ApiExceptionResponse(ex.Message, ex.StackTrace)
                            : new ApiExceptionResponse(ex.Message);

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            };
            var jsonResponse = JsonConvert.SerializeObject(response, jsonSettings);
            _logger.Here().Error("{@InternalServerError} - {@response}", ErrorCodes.InternalServerError, jsonResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
