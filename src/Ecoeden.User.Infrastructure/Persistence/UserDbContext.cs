using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Infrastructure.Persistence
{
    public class UserDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserEntityConfiguration).Assembly);

            builder.Entity<RolePermission>()
            .HasKey(ck => new { ck.RoleId, ck.PermissionId });
        }

        public DbSet<ApplicationPermission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public async Task<bool> DoesTableExist(string tableName)
        {
            // Build the SQL query to check if the table exists
            string query = $"SELECT * FROM pg_tables";

            // Execute the raw SQL query
            var result =  await Database.ExecuteSqlRawAsync(query);
            return result == 1;
        }
    }
}
