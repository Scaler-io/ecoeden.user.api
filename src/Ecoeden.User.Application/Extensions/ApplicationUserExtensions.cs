using Ecoeden.User.Domain.Entities;

namespace Ecoeden.User.Application.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static IEnumerable<string> GetUserRoleMappings(this ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user entity is null");
            }

            return user.UserRoles.Select(r => r.Role.Name).ToList();
        }

        public static IEnumerable<string> GetUserPermissionMappings(this ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user entity is null");
            }

            var result = user.UserRoles.Select(r => r.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList()).FirstOrDefault();
            return result;
        }
    }
}
