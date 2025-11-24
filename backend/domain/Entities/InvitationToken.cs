using app.Domain.Enums;

namespace app.Domain.Entities;

public class InvitationToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    
    // Campos opcionais pré-preenchidos
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
