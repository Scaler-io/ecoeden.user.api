using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.EventBus;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Application.EventBus;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Infrastructure.Cache;
using Ecoeden.User.Domain.ConfigurationOptions.EventBus;
using Ecoeden.User.Infrastructure.EventBus;
using Ecoeden.User.Infrastructure.Factory;
using Ecoeden.User.Infrastructure.HealthStatus;
using Ecoeden.User.Infrastructure.HealthStatus.DbHealthStatus;
using Ecoeden.User.Infrastructure.Persistence;
using Ecoeden.User.Infrastructure.Persistence.Repositories;
using Ecoeden.User.Infrastructure.Persistence.Security;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoeden.User.Infrastructure.DI;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection ConfigureInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IDbTranscation, DbTransaction>();
        services.AddScoped<IUserRepository, UserRepository>();

        var appRootPath = Directory.GetCurrentDirectory();

        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(appRootPath, "keys")))
            .SetApplicationName("ecoeden");

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<UserDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<ConfirmationEmailTokenProvider<ApplicationUser>>("EmailConfirmationTokenProvider");

        // health check service registrations
        services.AddScoped<IHealthCheck, DbHealthCheck>();
        services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();

        // cache implementations
        services.AddMemoryCache();
        services.AddScoped<ICacheServiceFactory, CacheServiceFactory>();
        services.AddScoped<ICacheService, InMemoryCacheService>();

        services.AddScoped(typeof(IPublishService<,>), typeof(PublishService<,>));
        services.AddScoped<IPublishServiceFactory, PublishServiceFactory>();

        services.AddMassTransit(config =>
        {
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("users", false));
            config.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMq = configuration.GetSection("EventBus").Get<EventBusOptions>();
                cfg.Host(rabbitMq.Host, "/", host =>
                {
                    host.Username(rabbitMq.Username);
                    host.Password(rabbitMq.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
