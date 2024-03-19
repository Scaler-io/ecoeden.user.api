using Ecoeden.Swagger;
using Serilog;
using User.Api.DependecyInjections;

var builder = WebApplication.CreateBuilder(args);

var apiName = SwaggerConfiguration.ExtractApiNameFromEnvironmentVariable();
var apiDescription = builder.Configuration["ApiDescription"];
var apiHost = builder.Configuration["ApiOriginHost"];
var swaggerConfiguration = new SwaggerConfiguration(apiName, apiDescription, apiHost, builder.Environment.IsDevelopment());

builder.Services
    .ConfiguredServices(builder.Configuration, swaggerConfiguration)
    .ConfigureApplicationOptions(builder.Configuration);

var app = builder.Build()
    .ConfigurePipleine(swaggerConfiguration);

try
{
    app.Run();
}finally
{
    Log.CloseAndFlush();
}

