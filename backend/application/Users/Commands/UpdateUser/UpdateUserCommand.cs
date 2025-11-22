using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}
