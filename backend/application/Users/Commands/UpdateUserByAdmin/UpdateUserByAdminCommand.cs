using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Users.Commands.UpdateUserByAdmin;

public class UpdateUserByAdminCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int? Role { get; set; }
    public bool? EmailConfirmed { get; set; }
}
