using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Infrastructure.HealthStatus.DbHealthStatus
{
    public sealed class DbHealthCheck : IHealthCheck
    {
        private readonly UserDbContext _userDbContext;

        public DbHealthCheck(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
        {
            var result = await _userDbContext.Users.AnyAsync();
            return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
