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
        public bool IsActive { get; set; }
        public bool IsDefaultAdmin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; private set; } = "Default";
        public string UpdateBy { get; private set; } = "Default";
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        public void SetCreatedBy(string username) => CreatedBy = username;
        public void SetUpdatedBy(string username) => UpdateBy = username;
        public void SetUpdationTime() => UpdatedAt = DateTime.UtcNow;
        public void ToggleVisibility() => IsActive = !IsActive;
    }
}
