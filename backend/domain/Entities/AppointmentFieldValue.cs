namespace app.Domain.Entities;

public class AppointmentFieldValue : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    
    public int SpecialtyFieldId { get; set; }
    public SpecialtyField SpecialtyField { get; set; } = null!;
    
    public string FieldValue { get; set; } = string.Empty; // Armazena o valor como string (pode ser JSON para valores complexos)
}
