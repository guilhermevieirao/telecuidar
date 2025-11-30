using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using app.Domain.Entities;
using app.Domain.Enums;
using System.Reflection;
using System.Text.Json;

namespace app.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private int? _currentUserId;
    private string? _ipAddress;
    private string? _userAgent;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<InvitationToken> InvitationTokens { get; set; }
    public DbSet<FileUpload> FileUploads { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<UserSpecialty> UserSpecialties { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public void SetAuditInfo(int? userId, string? ipAddress = null, string? userAgent = null)
    {
        _currentUserId = userId;
        _ipAddress = ipAddress;
        _userAgent = userAgent;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            // Não auditar AuditLogs e entidades que não são BaseEntity
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                UserId = _currentUserId,
                IpAddress = _ipAddress,
                UserAgent = _userAgent
            };

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                
                // Não auditar propriedades sensíveis
                if (propertyName == "PasswordHash")
                    continue;

                if (entry.State == EntityState.Added)
                {
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                }
                else if (entry.State == EntityState.Modified && property.IsModified)
                {
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                }
            }
        }

        // Para cada auditEntry, adicionamos os AuditLogs
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }

        return auditEntries;
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = default)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            // Atualizar EntityId para entidades Added
            if (auditEntry.EntityId == null && auditEntry.Entry.State == EntityState.Added)
            {
                var idProperty = auditEntry.Entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
                if (idProperty != null)
                {
                    auditEntry.EntityId = (int?)idProperty.CurrentValue;
                }
            }
        }

        return Task.CompletedTask;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public int? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();

    public AuditLog ToAuditLog()
    {
        var entityName = Entry.Entity.GetType().Name;
        var action = Entry.State switch
        {
            EntityState.Added => "CREATE",
            EntityState.Modified => "UPDATE",
            EntityState.Deleted => "DELETE",
            _ => "UNKNOWN"
        };

        // Tentar obter o ID da entidade
        int? entityId = null;
        var idProperty = Entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
        if (idProperty != null && idProperty.CurrentValue != null)
        {
            entityId = (int)idProperty.CurrentValue;
        }

        var auditLog = new AuditLog
        {
            UserId = UserId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            IpAddress = IpAddress,
            UserAgent = UserAgent,
            CreatedAt = DateTime.UtcNow
        };

        if (OldValues.Count > 0)
        {
            auditLog.OldValues = JsonSerializer.Serialize(OldValues);
        }

        if (NewValues.Count > 0)
        {
            auditLog.NewValues = JsonSerializer.Serialize(NewValues);
        }

        return auditLog;
    }
}