using Ecoeden.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoeden.User.Infrastructure.Persistence.Configurations
{
    public sealed class ApplicationPermissionEntityConfiguration : IEntityTypeConfiguration<ApplicationPermission>
    {
        public void Configure(EntityTypeBuilder<ApplicationPermission> builder)
        {
            builder.HasMany(rp => rp.RolePermissions)
                .WithOne(p => p.Permission)
                .HasForeignKey(fk => fk.PermissionId)
                .IsRequired();
        }
    }
}
