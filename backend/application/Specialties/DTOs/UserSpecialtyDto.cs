namespace app.Application.Specialties.DTOs;

public class UserSpecialtyDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SpecialtyId { get; set; }
    public string SpecialtyName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public DateTime AssignedAt { get; set; }
}
