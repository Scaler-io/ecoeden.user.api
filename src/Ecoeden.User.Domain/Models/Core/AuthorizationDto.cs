using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class AuthorizationDto
    {
        public IList<Role> Roles { get; set; }
        public IList<string> Permissions { get; set; }
        public string Token { get; set; }
    }
}