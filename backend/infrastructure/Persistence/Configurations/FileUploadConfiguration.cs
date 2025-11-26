using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using app.Domain.Entities;

namespace app.Infrastructure.Persistence.Configurations;

public class FileUploadConfiguration : IEntityTypeConfiguration<FileUpload>
{
    public void Configure(EntityTypeBuilder<FileUpload> builder)
    {
        builder.ToTable("FileUploads");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.FileCategory)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        // Relacionamento com usuário que fez upload
        builder.HasOne(f => f.UploadedByUser)
            .WithMany()
            .HasForeignKey(f => f.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com usuário relacionado (opcional)
        builder.HasOne(f => f.RelatedUser)
            .WithMany()
            .HasForeignKey(f => f.RelatedUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Índices para performance
        builder.HasIndex(f => f.UploadedByUserId);
        builder.HasIndex(f => f.RelatedUserId);
        builder.HasIndex(f => f.FileCategory);
        builder.HasIndex(f => f.CreatedAt);
    }
}
