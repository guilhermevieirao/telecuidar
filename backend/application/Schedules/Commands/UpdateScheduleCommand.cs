using MediatR;
using app.Application.Common.Models;
using app.Application.Schedules.DTOs;

namespace app.Application.Schedules.Commands;

public class UpdateScheduleCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public List<CreateScheduleDayDto> ScheduleDays { get; set; } = new();
}
