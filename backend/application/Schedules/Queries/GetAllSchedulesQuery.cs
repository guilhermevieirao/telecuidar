using MediatR;
using app.Application.Common.Models;
using app.Application.Schedules.DTOs;

namespace app.Application.Schedules.Queries;

public class GetAllSchedulesQuery : IRequest<Result<List<ScheduleDto>>>
{
    public int? ProfessionalId { get; set; }
    public bool? IsActive { get; set; }
}
