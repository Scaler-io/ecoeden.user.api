using Ecoeden.User.Application.EventBus;
using Ecoeden.User.Domain.Events;

namespace Ecoeden.User.Application.Contracts.EventBus;

public interface IPublishServiceFactory
{
    IPublishService<T, TEvent> CreatePublishService<T, TEvent>()
        where T : class
        where TEvent : IPublishable;
}
