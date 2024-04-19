using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Application.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static IEnumerable<string> GetUserRoleMappings(this ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException("user entity is null");
            }

            return user.UserRoles.Select(r => r.Role.Name).ToList();
        }

        public static IEnumerable<string> GetUserPermissionMappings(this ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException("user entity is null");
            }

            var result = user.UserRoles.SelectMany(r => r.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList()).Distinct();
            return result;
        }

        public static long GetUserRolesCount(this ApplicationUser user)
        {
            if(user is null ) throw new ArgumentNullException("user entity is null");
            return user.UserRoles.Count();
        }

        public static bool IsAdmin(this ApplicationUser user) => GetUserRoleMappings(user).Contains(Roles.Admin.ToString());
    }
}
