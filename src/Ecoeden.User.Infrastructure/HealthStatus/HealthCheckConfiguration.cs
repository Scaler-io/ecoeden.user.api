using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Microsoft.Extensions.Options;
using User.Infrastructure.ConfigurationOptions.App;

namespace Ecoeden.User.Infrastructure.HealthStatus
{
    public sealed class HealthCheckConfiguration : IHealthCheckConfiguration
    {
        private readonly ILogger _logger;
        public IRunHealthChecks HealthRunner { get; }

        public int HealthCheckTimeOutInSeconds { get; }

        public HealthCheckConfiguration(IEnumerable<IHealthCheck> healthChecks, ILogger logger, IOptions<AppOption> configuration)
        {
            _logger = logger;
            HealthRunner = AppMetricsHealth
                .CreateDefaultBuilder()
                .HealthChecks
                .AddChecks(healthChecks.Select(CreateHealthCheck))
                .Build()
                .HealthCheckRunner;
        }

        private HealthCheck CreateHealthCheck(IHealthCheck health)
        {
            return new HealthCheck(
                    health.GetType().Name,
                    async () =>
                    {
                        try
                        {
                            return await health.CheckIsHealthyAsync();
                        }
                        catch(Exception ex)
                        {
                            _logger.Warning("{Exception}", $"Healthcheck Failed: {ex}");
                            return HealthCheckResult.Unhealthy();
                        }
                    });
        }
    }
}
