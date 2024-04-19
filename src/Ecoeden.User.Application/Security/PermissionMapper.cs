using Ecoeden.User.Application.Contracts.Security;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Ecoeden.User.Application.Security
{
    public sealed class PermissionMapper : IPermissionMapper
    {
        public readonly Dictionary<ApiAccess, List<string>> _map = new();

        public PermissionMapper(RoleManager<ApplicationRole> roleManager)
        {
            var permissions = roleManager.Roles
                .SelectMany(r => r.RolePermissions.Select(rp => rp.Permission.Name).ToList())
                .ToList()
                .Distinct();

            foreach(var permission in permissions)
            {
                string[] parts = permission.Split(':');

                if (parts[0].Contains('_')) parts[0] = parts[0].Replace("_", "");

                if (parts.Length == 2 && Enum.TryParse<ApiAccess>(ToPascalCase(parts[0], parts[1]), out ApiAccess apiAccess))
                {
                    if (!_map.ContainsKey(apiAccess))
                    {
                        _map[apiAccess] = new List<string>();
                    }
                    _map[apiAccess].Add(permission);
                }
                
            }
        }

        public static string ToPascalCase(string first, string second)
        {
            return $"{Char.ToUpper(first[0])}{first.Substring(1)}{Char.ToUpper(second[0])}{second.Substring(1)}";
        }      

        public List<string> GetPermissionsForRole(ApiAccess role)
        {
            return _map[role];
        }
    }
}
