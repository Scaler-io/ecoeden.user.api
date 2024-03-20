namespace Ecoeden.User.Application.Contracts.HealthStatus
{
    public interface IHealthCheck
    {
        ValueTask<App.Metrics.Health.HealthCheckResult> CheckIsHealthyAsync();
    }
}
