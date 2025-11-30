namespace app.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public User Patient { get; set; } = null!;
    
    public int? ProfessionalId { get; set; }
    public User? Professional { get; set; }
    
    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
    
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public int DurationMinutes { get; set; }
    
    public string Status { get; set; } = "Agendado"; // Agendado, Confirmado, Cancelado, Concluído
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public string? MeetingRoomId { get; set; } // ID da sala de videochamada Jitsi
    
    public string? Notes { get; set; }
}
