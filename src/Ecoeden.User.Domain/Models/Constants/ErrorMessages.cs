using Ecoeden.User.Domain.Models.Core;

namespace Ecoeden.User.Domain.Models.Constants
{
    public static class ErrorMessages
    {
        public const string NotFound = "Not Found";
        public const string InternalServerError = "Internal Server Error";
        public const string BadRequest = "Bad Request";
        public const string Operationfailed = "Operation Failed";
        public const string Unauthorized = "Unauthorized";
        public const string NotAllowed = "Operation Not Allowed";

        public static ApiError UserNameRequired = new() { Code = "1000", Message = "Username is required" };
        public static ApiError FirstNameRequired = new() { Code = "1002", Message = "First name is required" };
        public static ApiError LastNameRequired = new() { Code = "1003", Message = "Last name is required" };
        public static ApiError EmailRequired = new() { Code = "1004", Message = "Email is required" };
        public static ApiError ValidEmail = new() { Code = "1005", Message = "Email is not valid" };
        public static ApiError RoleRequired = new() { Code = "1006", Message = "Atleast one role is required" };
        public static ApiError UserIdRequired = new() { Code = "1007", Message = "User id is rrequired for new role assignment" };
    }
}
