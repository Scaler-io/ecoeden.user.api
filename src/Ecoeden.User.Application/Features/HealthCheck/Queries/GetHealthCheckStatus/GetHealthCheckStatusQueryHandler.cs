using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.HealthCheck;
using MediatR;

namespace Ecoeden.User.Application.Features.HealthCheck.Queries.GetHealthCheckStatus
{
    public class GetHealthCheckStatusQueryHandler : IRequestHandler<GetHealthCheckStatusQuery, Result<HealthCheckResponse>>
    {
        private readonly ILogger _logger;
        private readonly IHealthCheckConfiguration _healthCheck;

        public GetHealthCheckStatusQueryHandler(ILogger logger, IHealthCheckConfiguration healthCheck)
        {
            _logger = logger;
            _healthCheck = healthCheck;
        }

        public async Task<Result<HealthCheckResponse>> Handle(GetHealthCheckStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();
            var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_healthCheck.HealthCheckTimeOutInSeconds));
            var response = await _healthCheck.HealthRunner.ReadAsync(timeoutTokenSource.Token);

            var isHealthy = response.Status == HealthCheckStatus.Healthy;

            if (!isHealthy)
            {
                foreach(var result in response.Results)
                {
                    _logger.Here().WithCorrelationId(request.CorrelationId)
                        .Error("{Message}", $"health check: {result.Name} : {result.Check.Status}");
                }
            }
            _logger.Here().WithCorrelationId(request.CorrelationId).Information("{Message}", $"health check completed. Status: {response.Status}");

            var healthCheckResponse = new HealthCheckResponse
            {
                IsHealthy = isHealthy
            };

            _logger.Here().MethodExited();

            return Result<HealthCheckResponse>.Success(healthCheckResponse);
        }
    }
}
