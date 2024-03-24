using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Application.Contracts.Persistence;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Infrastructure.HealthStatus;
using Ecoeden.User.Infrastructure.Persistence;
using Ecoeden.User.Infrastructure.Repositories;
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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // health check service registrations
            services.AddScoped<IHealthCheck, BaseRepository<ApplicationUser>>();

            services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();
            return services;
        }
    }
}
