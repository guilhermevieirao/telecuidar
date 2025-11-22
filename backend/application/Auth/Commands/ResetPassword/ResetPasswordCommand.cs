using MediatR;
using app.Application.Common.Models;

namespace app.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
