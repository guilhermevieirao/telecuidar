namespace app.Application.Appointments.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public int SpecialtyId { get; set; }
    public string SpecialtyName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AvailableSpecialtyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int AvailableProfessionalsCount { get; set; }
}

public class AvailableDateDto
{
    public DateTime Date { get; set; }
    public string DateFormatted { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public int AvailableSlotsCount { get; set; }
}

public class AvailableTimeSlotDto
{
    public string Time { get; set; } = string.Empty;
    public List<AvailableProfessionalDto> Professionals { get; set; } = new();
}

public class AvailableProfessionalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}
