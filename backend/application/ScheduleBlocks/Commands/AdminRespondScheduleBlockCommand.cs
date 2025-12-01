using MediatR;
using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;

namespace app.Application.ScheduleBlocks.Commands;

public class AdminRespondScheduleBlockCommand : IRequest<Result<ScheduleBlockDto>>
{
    public int BlockId { get; set; }
    public int AdminId { get; set; }
    public bool Accept { get; set; }
    public string Justification { get; set; } = string.Empty;
}
