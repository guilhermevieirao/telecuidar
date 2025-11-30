using MediatR;
using app.Application.Common.Models;

namespace app.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result<string>>
{
    public int UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
