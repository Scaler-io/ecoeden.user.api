using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Application.Extensions;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;

namespace Ecoeden.User.Infrastructure.HealthStatus
{
    public sealed class HealthCheckConfiguration : IHealthCheckConfiguration
    {
        //private readonly ILogger _logger;
        public IRunHealthChecks HealthRunner { get; }

        public int HealthCheckTimeOutInSeconds { get; }

        public HealthCheckConfiguration(IEnumerable<IHealthCheck> healthChecks, IOptions<AppOption> configuration)
        {
            HealthCheckTimeOutInSeconds = configuration.Value.HealthCheckTimeOutInSeconds;

            HealthRunner = AppMetricsHealth
                .CreateDefaultBuilder()
                .HealthChecks
                .AddChecks(healthChecks.Select(CreateHealthCheck))
                .Build()
                .HealthCheckRunner;
            //_logger = logger;
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
                        catch (Exception ex)
                        {
                            //_logger.Here().Error("{Exception}:", $"Health check failure {ex}");
                            return HealthCheckResult.Unhealthy();
                        }
                    });
        }
    }
}
