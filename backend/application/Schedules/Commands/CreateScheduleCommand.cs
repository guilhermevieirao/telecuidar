using MediatR;
using app.Application.Common.Models;
using app.Application.Schedules.DTOs;

namespace app.Application.Schedules.Commands;

public class CreateScheduleCommand : IRequest<Result<int>>
{
    public int ProfessionalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateScheduleDayDto> ScheduleDays { get; set; } = new();
    public int? CreatedByUserId { get; set; }
}

public class CreateScheduleDayDto
{
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int AppointmentDuration { get; set; }
    public int IntervalBetweenAppointments { get; set; } = 0;
    public string? BreakStartTime { get; set; }
    public string? BreakEndTime { get; set; }
}
