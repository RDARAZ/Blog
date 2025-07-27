using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Description)
            .HasMaxLength(500);
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Role(Role.RoleName.User)
            {
                Id = 1,
                Description = "Standard user with basic permissions",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role(Role.RoleName.Admin)
            {
                Id = 2,
                Description = "Administrator with full permissions",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
