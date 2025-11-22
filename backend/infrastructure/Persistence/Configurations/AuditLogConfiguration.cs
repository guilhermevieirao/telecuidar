using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using app.Domain.Entities;

namespace app.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Action)
            .IsRequired();

        builder.Property(e => e.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EntityId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Details)
            .HasMaxLength(1000);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => new { e.EntityName, e.EntityId });
    }
}
