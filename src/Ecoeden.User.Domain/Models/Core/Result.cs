using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public ErrorCodes? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public static Result<T> Success(T value)
        {
            return new Result<T> { IsSuccess = true, Value = value };
        }

        public static Result<T> Failure(ErrorCodes errorCode, string errorMessage = null)
        {
            return new Result<T> { IsSuccess = false, ErrorCode = errorCode, ErrorMessage = errorMessage };
        }
    }
}
