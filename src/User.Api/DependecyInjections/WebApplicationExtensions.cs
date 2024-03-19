using Asp.Versioning.ApiExplorer;
using Ecoeden.Swagger;
using User.Api.Middlewares;

namespace User.Api.DependecyInjections
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipleine(this WebApplication app, SwaggerConfiguration swaggerConfiguration)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(SwaggerConfiguration.SetupSwaggerOptions);
                app.UseSwaggerUI(options =>
                {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    swaggerConfiguration.SetupSwaggerUiOptions(options, provider);
                });
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestLoggerMiddleware>()
                .UseMiddleware<CorrelationHeaderEnricher>()
                .UseMiddleware<GlobalExceptionMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
