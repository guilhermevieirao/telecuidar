using MediatR;
using app.Application.Common.Models;

namespace app.Application.Specialties.Commands;

public class DeleteSpecialtyCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}
