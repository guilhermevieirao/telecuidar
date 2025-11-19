using MediatR;
using app.Application.Common.Models;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<List<UserDto>>>
{
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
}