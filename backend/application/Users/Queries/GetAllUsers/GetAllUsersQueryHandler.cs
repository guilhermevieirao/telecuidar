using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserListDto>>>
{
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public GetAllUsersQueryHandler(IRepository<Domain.Entities.User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
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

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);
            
            var userDtos = users.Select(user => new UserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = (int)user.Role,
                RoleName = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                EmailConfirmed = user.EmailConfirmed
            }).ToList();

            return Result<List<UserListDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            return Result<List<UserListDto>>.Failure("Erro ao buscar usuários", new List<string> { ex.Message });
        }
    }
}