namespace Ecoeden.User.Domain.Entities
{
    public class ApplicationPermission
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
