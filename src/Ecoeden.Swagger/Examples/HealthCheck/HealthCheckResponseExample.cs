using Ecoeden.User.Domain.Models.Responses.HealthCheck;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples.HealthCheck
{
    public class HealthCheckResponseExample : IExamplesProvider<HealthCheckResponse>
    {
        public HealthCheckResponse GetExamples()
        {
            return new HealthCheckResponse
            {
                IsHealthy = true
            };
        }
    }
}
