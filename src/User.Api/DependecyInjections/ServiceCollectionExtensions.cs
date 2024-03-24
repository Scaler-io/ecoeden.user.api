using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Ecoeden.Swagger;
using Ecoeden.Swagger.Examples.HealthCheck;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;

namespace User.Api.DependecyInjections
{
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
}
