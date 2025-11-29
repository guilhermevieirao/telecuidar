using MediatR;
using app.Application.Common.Models;

namespace app.Application.Schedules.Commands;

public class DeleteScheduleCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}
