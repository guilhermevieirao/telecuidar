using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Enums;

namespace app.Application.Auth.Commands.Register;

public class RegisterCommand : IRequest<Result<UserDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Paciente;
    public string? InvitationToken { get; set; }
}
