using MediatR;
using app.Application.Common.Models;

namespace app.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }

    public DeleteUserCommand(Guid id)
    {
        Id = id;
    }
}
