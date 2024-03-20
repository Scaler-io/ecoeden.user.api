using App.Metrics.Health;

namespace Ecoeden.User.Application.Contracts.HealthStatus
{
    public interface IHealthCheckConfiguration
    {
        IRunHealthChecks HealthRunner { get; }
        int HealthCheckTimeOutInSeconds { get; }
    }
}
