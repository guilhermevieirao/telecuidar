using app.Domain.Enums;

namespace app.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
