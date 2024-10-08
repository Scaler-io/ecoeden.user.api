﻿using Ecoeden.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoeden.User.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasMany(r => r.UserRoles)
           .WithOne(u => u.User)
           .HasForeignKey(fk => fk.UserId)
           .IsRequired();
    }
}
