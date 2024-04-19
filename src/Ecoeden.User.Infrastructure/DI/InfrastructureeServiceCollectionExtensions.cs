using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Infrastructure.Cache;
using Ecoeden.User.Infrastructure.Factory;
using Ecoeden.User.Infrastructure.HealthStatus;
using Ecoeden.User.Infrastructure.HealthStatus.DbHealthStatus;
using Ecoeden.User.Infrastructure.Persistence;
using Ecoeden.User.Infrastructure.Persistence.Repositories;
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

            services.AddScoped<IDbTranscation, DbTransaction>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();

            // health check service registrations
            services.AddScoped<IHealthCheck, DbHealthCheck>();
            services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();

            // cache implementations
            services.AddMemoryCache();
            services.AddScoped<ICacheServiceFactory, CacheServiceFactory>();
            services.AddScoped<ICacheService, InMemoryCacheService>();

            return services;
        }
    }
}
