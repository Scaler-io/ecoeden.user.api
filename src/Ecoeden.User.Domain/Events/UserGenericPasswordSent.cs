using Ecoeden.User.Domain.Events;
using Ecoeden.User.Domain.Models.Enums;

namespace Contracts.Events;
public sealed class UserGenericPasswordSent : NotificationEvent
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string DefaultPassword { get; set; }
    protected override NotificationType NotificationType { get; set; } = NotificationType.UserGenericPasswordSent;
}
