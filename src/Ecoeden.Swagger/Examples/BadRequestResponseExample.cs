using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples
{
    public sealed class BadRequestResponseExample : IExamplesProvider<ApiResponse>
    {
        public ApiResponse GetExamples()
        {
            return new ApiResponse(ErrorCodes.BadRequest)
            {
                ErrorMessage = ErrorMessages.BadRequest
            };
        }
    }
}
