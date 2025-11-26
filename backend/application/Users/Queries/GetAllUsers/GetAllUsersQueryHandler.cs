using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedResult<UserListDto>>>
{
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public GetAllUsersQueryHandler(IRepository<Domain.Entities.User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedResult<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _userRepository.GetQueryable();

            // Filtrar por role se especificado
            if (request.Role.HasValue)
            {
                query = query.Where(u => (int)u.Role == request.Role.Value);
            }

            // Filtrar por termo de busca
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))
                );
            }

            // Aplicar ordenação
            var sortBy = request.SortBy?.ToLower() ?? "createdat";
            var isDescending = request.SortDirection?.ToLower() == "desc";

            query = sortBy switch
            {
                "id" => isDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
                "fullname" => isDescending ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName) : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                "isactive" => isDescending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
                "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            // Obter contagem total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            var userDtos = users.Select(user => new UserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = (int)user.Role,
                RoleName = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive
            }).ToList();

            var pagedResult = PagedResult<UserListDto>.Create(userDtos, totalCount, request.PageNumber, request.PageSize);
            
            return Result<PagedResult<UserListDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<UserListDto>>.Failure("Erro ao buscar usuários", new List<string> { ex.Message });
        }
    }
}