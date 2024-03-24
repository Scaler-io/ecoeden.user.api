using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples
{
    public sealed class BadRequestResponseExample : IExamplesProvider<ApiExceptionResponse>
    {
        public ApiExceptionResponse GetExamples()
        {
            return new ApiExceptionResponse()
            {
                ErrorMessage = ErrorMessages.BadRequest
            };
        }
    }
}
