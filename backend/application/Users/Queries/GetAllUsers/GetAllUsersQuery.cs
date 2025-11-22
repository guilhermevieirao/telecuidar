using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<List<UserListDto>>>
{
    public int? Role { get; set; }
    public string? SearchTerm { get; set; }
}