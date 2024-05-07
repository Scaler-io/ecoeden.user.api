using AutoMapper;
using Ecoeden.User.Application.EventBus;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Events;
using MassTransit;

namespace Ecoeden.User.Infrastructure.EventBus;

public class PublishService<T, TEvent> : IPublishService<T, TEvent>
    where T : class
    where TEvent : NotificationEvent
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PublishService(IPublishEndpoint publishEndpoint, 
        IMapper mapper, 
        ILogger logger)
    {
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task PublishAsync(T message, string correlationId, object aditionalProperties = null)
    {
        var newEvent = _mapper.Map<TEvent>(message);
        newEvent.CorrelationId = correlationId;
        newEvent.AdditionalProperties = aditionalProperties;

        await _publishEndpoint.Publish(newEvent);

        _logger.Here() 
            .WithCorrelationId(correlationId)
            .Information("Successfully published {messageType} event message", typeof(TEvent).Name);
    }
}
