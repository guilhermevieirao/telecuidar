using app.Domain.Enums;

namespace app.Application.Admin.DTOs;

public class InvitationDetailsDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
}
