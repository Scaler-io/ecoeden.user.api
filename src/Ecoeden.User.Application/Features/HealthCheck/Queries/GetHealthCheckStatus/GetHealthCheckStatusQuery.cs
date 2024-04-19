using Ecoeden.User.Application.Behaviors;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.HealthCheck;
using MediatR;

namespace Ecoeden.User.Application.Features.HealthCheck.Queries.GetHealthCheckStatus
{
    public class GetHealthCheckStatusQuery : IRequest<Result<HealthCheckResponse>>, ISkipPiplineBehavior
    {
        public string CorrelationId { get; set; }

        public GetHealthCheckStatusQuery(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
