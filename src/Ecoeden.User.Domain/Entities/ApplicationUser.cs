using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecoeden.User.Domain.Entities
{
    [NotMapped]
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public DateTime LastLogin { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; private set; } = "Default";
        public string UpdateBy { get; private set; } = "Default";

        public void SetCreatedBy(string username) => CreatedBy = username;
        public void SetUpdatedBy(string username) => UpdateBy = username;
        public void setUpdationTime() => UpdatedAt = DateTime.UtcNow;
    }
}
