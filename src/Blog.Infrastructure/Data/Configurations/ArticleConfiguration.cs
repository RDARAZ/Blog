using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Content)
            .IsRequired()
            .HasColumnType("ntext");

        builder.Property(a => a.Summary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Articles_Status");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_Articles_CreatedAt");

        builder.HasIndex(a => a.AuthorId)
            .HasDatabaseName("IX_Articles_AuthorId");
    }
}
