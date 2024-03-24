using Ecoeden.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoeden.User.Infrastructure.Persistence.Configurations
{
    public sealed class ApplicationRoleEntityConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(fk => fk.RoleId)
                .IsRequired();

            builder.HasMany(rp => rp.RolePermissions)
                .WithOne(r => r.Role)
                .HasForeignKey(fk => fk.RoleId)
                .IsRequired();
        }
    }
}
