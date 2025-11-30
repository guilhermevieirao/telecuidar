namespace app.Application.Schedules.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public List<ScheduleDayDto> ScheduleDays { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByName { get; set; }
}

public class ScheduleDayDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int AppointmentDuration { get; set; }
    public int IntervalBetweenAppointments { get; set; }
    public string? BreakStartTime { get; set; }
    public string? BreakEndTime { get; set; }
}
