namespace app.Domain.Entities;

public class Specialty : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; } // Emoji ou ícone da especialidade
    
    // Navigation properties
    public ICollection<UserSpecialty> UserSpecialties { get; set; } = new List<UserSpecialty>();
}
