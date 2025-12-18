using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<PaginatedUsersDto> GetUsersAsync(int page, int pageSize, string? search, string? role, string? status)
    {
        var query = _context.Users.Include(u => u.Specialty).AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.Name.Contains(search) ||
                u.LastName.Contains(search) ||
                u.Email.Contains(search) ||
                u.Cpf.Contains(search));
        }

        if (!string.IsNullOrEmpty(role) && role.ToLower() != "all")
        {
            if (Enum.TryParse<UserRole>(role, true, out var userRole))
            {
                query = query.Where(u => u.Role == userRole);
            }
        }

        if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
        {
            if (Enum.TryParse<UserStatus>(status, true, out var userStatus))
            {
                query = query.Where(u => u.Status == userStatus);
            }
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                LastName = u.LastName,
                Cpf = u.Cpf,
                Phone = u.Phone,
                Avatar = u.Avatar,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),
                EmailVerified = u.EmailVerified,
                SpecialtyId = u.SpecialtyId,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return new PaginatedUsersDto
        {
            Data = users,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Specialty)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            LastName = user.LastName,
            Cpf = user.Cpf,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            EmailVerified = user.EmailVerified,
            SpecialtyId = user.SpecialtyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Validate email doesn't exist
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("Email already in use");
        }

        // Validate CPF doesn't exist
        if (await _context.Users.AnyAsync(u => u.Cpf == dto.Cpf))
        {
            throw new InvalidOperationException("CPF already in use");
        }

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var userRole))
        {
            throw new InvalidOperationException("Invalid role");
        }

        var user = new User
        {
            Email = dto.Email,
            Name = dto.Name,
            LastName = dto.LastName,
            Cpf = dto.Cpf,
            Phone = dto.Phone,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Role = userRole,
            Status = UserStatus.Active,
            EmailVerified = false,
            SpecialtyId = dto.SpecialtyId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            LastName = user.LastName,
            Cpf = user.Cpf,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            EmailVerified = user.EmailVerified,
            SpecialtyId = user.SpecialtyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.LastName))
            user.LastName = dto.LastName;

        if (dto.Phone != null)
            user.Phone = dto.Phone;

        if (dto.Avatar != null)
            user.Avatar = dto.Avatar;

        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<UserStatus>(dto.Status, true, out var status))
            user.Status = status;

        if (dto.SpecialtyId.HasValue)
            user.SpecialtyId = dto.SpecialtyId;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            LastName = user.LastName,
            Cpf = user.Cpf,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            EmailVerified = user.EmailVerified,
            SpecialtyId = user.SpecialtyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
