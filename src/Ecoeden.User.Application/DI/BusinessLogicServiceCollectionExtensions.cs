using Ecoeden.User.Application.Contracts.Security;
using Ecoeden.User.Application.Security;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ecoeden.User.Application.DI
{
    public static class BusinessLogicServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBusinessLogicServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<IPermissionMapper, PermissionMapper>();
            return services;
        }
    }
}
