using Ecoeden.User.Application.Contracts.EventBus;
using Ecoeden.User.Application.EventBus;
using Ecoeden.User.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoeden.User.Infrastructure.EventBus;

public class PublishServiceFactory : IPublishServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PublishServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPublishService<T, TEvent> CreatePublishService<T, TEvent>()
        where T : class
        where TEvent : IPublishable
    {
        return _serviceProvider.GetRequiredService<IPublishService<T, TEvent>>();
    }
}
