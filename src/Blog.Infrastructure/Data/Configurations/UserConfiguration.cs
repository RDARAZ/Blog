using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint("CK_User_Age", "Age IS NULL OR (Age >= 13 AND Age <= 120)");
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Salt)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.Age);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.HasMany(u => u.Articles)
            .WithOne(a => a.Author)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
