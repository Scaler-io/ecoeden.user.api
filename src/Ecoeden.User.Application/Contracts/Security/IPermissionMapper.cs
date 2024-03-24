using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Application.Contracts.Security
{
    public interface IPermissionMapper
    {
        List<string> GetPermissionsForRole(ApiAccess role);
    }
}
