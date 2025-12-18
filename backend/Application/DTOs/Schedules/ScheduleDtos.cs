namespace Application.DTOs.Schedules;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateScheduleDto
{
    public Guid ProfessionalId { get; set; }
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; } = 30;
}

public class UpdateScheduleDto
{
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int? SlotDurationMinutes { get; set; }
    public bool? IsActive { get; set; }
}

public class AvailableSlotDto
{
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class ProfessionalAvailabilityDto
{
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public List<AvailableSlotDto> Slots { get; set; } = new();
}

public class PaginatedSchedulesDto
{
    public List<ScheduleDto> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
