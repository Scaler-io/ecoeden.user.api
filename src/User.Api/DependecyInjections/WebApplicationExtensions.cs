﻿using Asp.Versioning.ApiExplorer;
using Ecoeden.Swagger;
using User.Api.Middlewares;

namespace User.Api.DependecyInjections;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipleine(this WebApplication app, SwaggerConfiguration swaggerConfiguration)
    {
        app.UseSwagger(SwaggerConfiguration.SetupSwaggerOptions);
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            swaggerConfiguration.SetupSwaggerUiOptions(options, provider);
        });

        app.UseHttpsRedirection();

        app.UseMiddleware<CorrelationHeaderEnricher>()
            .UseMiddleware<RequestLoggerMiddleware>()
            .UseMiddleware<GlobalExceptionMiddleware>();


        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseCors("ecoedencors");

        return app;
    }
}
