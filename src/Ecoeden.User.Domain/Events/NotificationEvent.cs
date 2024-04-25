using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Events
{
    public abstract class NotificationEvent
    {
        protected abstract NotificationType NotificationType { get; set; }
    }
}
