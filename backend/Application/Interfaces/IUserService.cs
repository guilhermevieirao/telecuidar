using Application.DTOs.Users;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<PaginatedUsersDto> GetUsersAsync(int page, int pageSize, string? search, string? role, string? status);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
}
