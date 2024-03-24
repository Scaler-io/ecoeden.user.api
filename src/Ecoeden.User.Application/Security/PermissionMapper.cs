using Ecoeden.User.Application.Contracts.Security;
using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Application.Security
{
    public sealed class PermissionMapper : IPermissionMapper
    {

        public readonly Dictionary<ApiAccess, List<string>> _map;

        public PermissionMapper()
        {
            _map = new Dictionary<ApiAccess, List<string>>();
            _map.Add(ApiAccess.SettingsWrite, GetSettingsWritePermissions());
            _map.Add(ApiAccess.ReportRead, GetReportReadPermissions());
            _map.Add(ApiAccess.UserManagementRead, GetUserManagementReadPermissions());
            _map.Add(ApiAccess.RoleRead, GetRoleReadPermissions());
            _map.Add(ApiAccess.RoleWrite, GetRoleWritePermissions());
            _map.Add(ApiAccess.UserManagementWrite, GetUserManagementWritePermissions());
            _map.Add(ApiAccess.InventoryRead, GetInventoryReadPermissions());
            _map.Add(ApiAccess.UserWrite, GetUserWritePermissions());
            _map.Add(ApiAccess.ReportWrite, GetReportWritePermissions());
            _map.Add(ApiAccess.UserRead, GetUserReadPermissions());
            _map.Add(ApiAccess.SettingsRead, GetSettingsReadPermissions());
            _map.Add(ApiAccess.InventoryWrite, GetInventoryWritePermissions());
        }

        public List<string> GetPermissionsForRole(ApiAccess role)
        {
            return _map[role];
        }

        private List<string> GetSettingsWritePermissions()
        {
            return new List<string>
            {
                "settings:write",
                "settings:read"
            };
        }

        private List<string> GetReportWritePermissions()
        {
            return new List<string>
            {
                "report:write",
                "report:read"
            };
        }

        private List<string> GetUserManagementWritePermissions()
        {
            return new List<string>
            {
                "user_management:write",
                "user_management:read"
            };
        }

        private List<string> GetRoleWritePermissions()
        {
            return new List<string>
            {
                "role:write",
                "role:read"
            };
        }

        private List<string> GetUserWritePermissions()
        {
            return new List<string>
            {
                "user:write",
                "user:read"
            };
        }

        private List<string> GetInventoryWritePermissions()
        {
            return new List<string>
            {
                "inventory:write",
                "inventory:read"
            };
        }

        private List<string> GetSettingsReadPermissions()
        {
            return new List<string>
            {
                "settings:read"
            };
        }

        private List<string> GetReportReadPermissions()
        {
            return new List<string>
            {
                "report:read"
            };
        }

        private List<string> GetUserManagementReadPermissions()
        {
            return new List<string>
            {
                "user_management:read"
            };
        }

        private List<string> GetRoleReadPermissions()
        {
            return new List<string>
            {
                "role:read"
            };
        }

        private List<string> GetUserReadPermissions()
        {
            return new List<string>
            {
                "user:read"
            };
        }

        private List<string> GetInventoryReadPermissions()
        {
            return new List<string>
            {
                "inventory:read"
            };
        }

    }
}
