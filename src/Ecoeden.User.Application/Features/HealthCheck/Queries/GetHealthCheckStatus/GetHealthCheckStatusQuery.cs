using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.HealthCheck;
using MediatR;

namespace Ecoeden.User.Application.Features.HealthCheck.Queries.GetHealthCheckStatus
{
    public class GetHealthCheckStatusQuery : IRequest<Result<HealthCheckResponse>>
    {
        public string CorrelationId { get; set; }

        public GetHealthCheckStatusQuery(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
