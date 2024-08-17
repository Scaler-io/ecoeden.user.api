namespace Ecoeden.User.Domain.Events;

public interface IPublishable
{
    string CorrelationId { get; set; }
    object AdditionalProperties { get; set; }
}
