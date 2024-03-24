namespace Ecoeden.User.Domain.Entities
{
    public class RolePermission
    {
        public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }

        public string PermissionId { get; set; }
        public ApplicationPermission Permission { get; set; }
    }
}
