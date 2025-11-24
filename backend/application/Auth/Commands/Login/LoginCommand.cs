using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
