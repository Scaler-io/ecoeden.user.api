using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples
{
    public sealed class InternalServerResponseExample : IExamplesProvider<ApiExceptionResponse>
    {
        public ApiExceptionResponse GetExamples()
        {
            return new ApiExceptionResponse
            {
                ErrorMessage = ErrorMessages.InternalServerError
            };
        }
    }
}
