using app.Domain.Enums;

namespace app.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public UserRole Role { get; set; } = UserRole.Paciente;
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation properties
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    public ICollection<UserSpecialty> UserSpecialties { get; set; } = new List<UserSpecialty>();
}