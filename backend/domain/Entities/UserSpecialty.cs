namespace app.Domain.Entities;

public class UserSpecialty : BaseEntity
{
    public int UserId { get; set; }
    public int SpecialtyId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Specialty Specialty { get; set; } = null!;
}
