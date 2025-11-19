using MediatR;
using app.Application.Common.Models;
using app.Domain.Interfaces;

namespace app.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public GetAllUsersQueryHandler(IRepository<Domain.Entities.User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                LastLoginAt = user.LastLoginAt
            }).ToList();

            return Result<List<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            return Result<List<UserDto>>.Failure("Erro ao buscar usuários", new List<string> { ex.Message });
        }
    }
}