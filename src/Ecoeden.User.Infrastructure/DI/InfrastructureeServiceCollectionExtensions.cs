using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Infrastructure.HealthStatus;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoeden.User.Infrastructure.DI
{
    public static class InfrastructureeServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureInfraServices(this IServiceCollection services)
        {
            services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();
            return services;
        }
    }
}
