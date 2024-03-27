using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Application.Contracts.Persistence;
using Ecoeden.User.Application.Contracts.Persistence.Users;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Infrastructure.Cache;
using Ecoeden.User.Infrastructure.Factory;
using Ecoeden.User.Infrastructure.HealthStatus;
using Ecoeden.User.Infrastructure.Persistence;
using Ecoeden.User.Infrastructure.Repositories;
using Ecoeden.User.Infrastructure.Repositories.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoeden.User.Infrastructure.DI
{
    public static class InfrastructureeServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // health check service registrations
            services.AddScoped<IHealthCheck, BaseRepository<ApplicationUser>>();
            services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();

            // cache implementations
            services.AddMemoryCache();
            services.AddScoped<ICacheServiceFactory, CacheServiceFactory>();
            services.AddScoped<ICacheService, InMemoryCacheService>();

            return services;
        }
    }
}
