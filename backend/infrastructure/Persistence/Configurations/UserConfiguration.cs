using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using app.Domain.Entities;

namespace app.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(e => e.LastLoginAt)
            .IsRequired(false);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasDefaultValue(app.Domain.Enums.UserRole.Paciente)
            .HasSentinel(0);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(e => e.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Ignorar propriedade calculada
        builder.Ignore(e => e.FullName);

        // Relacionamentos
        builder.HasMany(e => e.PasswordResetTokens)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
