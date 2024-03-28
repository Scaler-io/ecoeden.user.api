using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples
{
    public sealed class NotFoundResponseExample : IExamplesProvider<ApiResponse>
    {
        public ApiResponse GetExamples()
        {
            return new ApiResponse(ErrorCodes.NotFound);
        }
    }
}
