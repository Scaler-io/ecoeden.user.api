using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Events;

public abstract class GenericEvent : IPublishable
{
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CorrelationId { get; set; }
    public Dictionary<string, string> AdditionalProperties { get; set; }
    protected abstract GenericEventType GenericEventType { get; set; }
}
