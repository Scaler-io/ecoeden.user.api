using Ecoeden.User.Domain.Models.Enums;

namespace User.Api.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute
    {
        public ApiPermission Permission { get; set; }
    }
}
