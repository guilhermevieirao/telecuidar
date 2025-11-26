namespace app.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info"; // "info", "success", "warning", "error"
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    
    // Relacionamento com usuário destinatário
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Relacionamento opcional com usuário que criou (pode ser sistema)
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    public string? RelatedEntityType { get; set; } // "Appointment", "Message", "FileUpload", etc
    public int? RelatedEntityId { get; set; }
}
