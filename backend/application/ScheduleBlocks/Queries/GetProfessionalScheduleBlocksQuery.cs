using MediatR;
using app.Application.ScheduleBlocks.DTOs;
using app.Application.Common.Models;

namespace app.Application.ScheduleBlocks.Queries;

public class GetProfessionalScheduleBlocksQuery : IRequest<Result<List<ScheduleBlockDto>>>
{
    public int ProfessionalId { get; set; }
}
