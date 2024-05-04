using Ecoeden.User.Application.Behaviors;
using Ecoeden.User.Application.Contracts.Security;
using Ecoeden.User.Application.Mappers;
using Ecoeden.User.Application.Security;
using Ecoeden.User.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ecoeden.User.Application.DI;

public static class BusinessLogicServiceCollectionExtensions
{
    public static IServiceCollection ConfigureBusinessLogicServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbTransactionBehavior<,>));

        services.AddSingleton<IPermissionMapper, PermissionMapper>(sp =>
        {
            using var scope = sp.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            return new PermissionMapper(roleManager);
        });

        // auto mapping
        services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
        // fluent validations
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
