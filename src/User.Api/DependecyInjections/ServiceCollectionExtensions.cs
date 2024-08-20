using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Ecoeden.Swagger;
using Ecoeden.Swagger.Examples.HealthCheck;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.ConfigurationOptions.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.Filters;
using User.Api.Services;

namespace User.Api.DependecyInjections;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfiguration configuration,
        SwaggerConfiguration swaggerConfiguration)
    {

        services.AddControllers()
            .AddNewtonsoftJson(config =>
            {
                config.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                config.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                config.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


        // swagger generation set up
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(option =>
        {
            option.DefaultApiVersion = ApiVersion.Default;
            option.ReportApiVersions = true;
            option.AssumeDefaultVersionWhenUnspecified = true;
        }).AddApiExplorer(option =>
        {
            option.GroupNameFormat = "'v'VVV";
            option.SubstituteApiVersionInUrl = true;
        });

        // handles api's default error validation model
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = HandleFrameworkValidationFailure();
        });

        // swagger setup
        services.AddSwaggerExamplesFromAssemblies(typeof(HealthCheckResponseExample).Assembly);
        services.AddSwaggerExamples();
        services.AddSwaggerGen(options =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            swaggerConfiguration.SetupSwaggerGenService(options, provider);
        });

        // configure identity
        var identityGroupAccess = configuration
            .GetSection("IdentityGroupAccess")
            .Get<IdentityGroupAccessOptions>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = identityGroupAccess.Audience;
            options.Authority = identityGroupAccess.Authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim("scope", "userapi:read")
                .RequireClaim("scope", "userapi:write")
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("ecoeden.user.api"))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter();
            });

        return services;
    }

    private static Func<ActionContext, IActionResult> HandleFrameworkValidationFailure()
    {
        return context =>
        {
            var errors = context.ModelState
                            .Where(m => m.Value.Errors.Count > 0)
                            .ToList();

            var validationError = new ApiValidationResponse
            {
                Errors = new List<FieldLevelError>()
            };

            foreach (var error in errors)
            {
                var fieldLevelError = new FieldLevelError
                {
                    Code = ErrorCodes.BadRequest.ToString(),
                    Field = error.Key,
                    Message = error.Value.Errors?.First().ErrorMessage
                };
                validationError.Errors.Add(fieldLevelError);
            }

            return new BadRequestObjectResult(validationError);
        };
    }
}
