using Ecoeden.User.Domain.Events;

namespace Ecoeden.User.Application.EventBus
{
    public interface IPublishService<T, TEvent>
        where T : class
        where TEvent : NotificationEvent
    {
        Task PublishAsync(T message, string correlationId, object additionalProperties = null);
    }
}
