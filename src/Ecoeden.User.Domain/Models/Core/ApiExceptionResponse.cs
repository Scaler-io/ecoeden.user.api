using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class ApiExceptionResponse : ApiResponse
    {
        public ApiExceptionResponse(string errorMessage = "", string stackTrace = "") 
            : base(ErrorCodes.InternalServerError)
        {
            ErrorMessage = errorMessage ?? GetDefaultMessage(Code);
            StackTrace = stackTrace;
        }

        public string StackTrace { get; set; }
    }
}
