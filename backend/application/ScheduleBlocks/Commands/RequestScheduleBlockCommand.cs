using MediatR;
using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;

namespace app.Application.ScheduleBlocks.Commands;

public class RequestScheduleBlockCommand : IRequest<Result<ScheduleBlockDto>>
{
    public int ProfessionalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}
