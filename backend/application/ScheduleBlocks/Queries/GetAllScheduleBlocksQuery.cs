using MediatR;
using app.Application.ScheduleBlocks.DTOs;
using app.Application.Common.Models;

namespace app.Application.ScheduleBlocks.Queries;

public class GetAllScheduleBlocksQuery : IRequest<Result<List<ScheduleBlockDto>>>
{
}
