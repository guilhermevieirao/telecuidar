using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<PagedResult<UserListDto>>>
{
    public int? Role { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}