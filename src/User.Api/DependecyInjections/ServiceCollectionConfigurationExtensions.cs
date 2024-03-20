using User.Api.ConfigurationOptions.Elastic;
using User.Api.ConfigurationOptions.Logging;
using User.Infrastructure.ConfigurationOptions.App;

namespace User.Api.DependecyInjections
{
    public static class ServiceCollectionConfigurationExtensions
    {
        public static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<AppOption>(configuration.GetSection(AppOption.OptionName));
            services.Configure<ElasticSearchOption>(configuration.GetSection(ElasticSearchOption.OptionName));
            services.Configure<LoggingOption>(configuration.GetSection(LoggingOption.OptionName));
            return services;
        }
    }
}
