using MediatR;
using app.Application.Common.Models;

namespace app.Application.Auth.Commands.RequestPasswordReset;

public class RequestPasswordResetCommand : IRequest<Result<bool>>
{
    public string Email { get; set; } = string.Empty;
}
