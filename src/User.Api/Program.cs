using Ecoeden.Swagger;
using Ecoeden.User.Infrastructure.DI;
using Serilog;
using User.Api;
using User.Api.DependecyInjections;
using Ecoeden.User.Application.DI;

var builder = WebApplication.CreateBuilder(args);

var apiName = SwaggerConfiguration.ExtractApiNameFromEnvironmentVariable();
var apiDescription = builder.Configuration["ApiDescription"];
var apiHost = builder.Configuration["ApiOriginHost"];
var swaggerConfiguration = new SwaggerConfiguration(apiName, apiDescription, apiHost, builder.Environment.IsDevelopment());

var logger = Logging.GetLogger(builder.Configuration, builder.Environment);
builder.Host.UseSerilog(logger);

builder.Services
    .ConfigureServices(builder.Configuration, swaggerConfiguration)
    .ConfigureInfraServices(builder.Configuration)
    .ConfigureApplicationOptions(builder.Configuration)
    .ConfigureBusinessLogicServices();

var app = builder.Build()
    .ConfigurePipleine(swaggerConfiguration);

try
{
    app.Run();
}finally
{
    Log.CloseAndFlush();
}

