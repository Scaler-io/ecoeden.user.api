using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Events
{
    public abstract class NotificationEvent
    {
        public DateTime CreatedAt { get; set; }
        public string CorrelationId { get; set; }
        public object? AdditionalProperties { get; set; }
        protected abstract NotificationType NotificationType { get; set; }
    }
}
