using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;

    public GetUserByIdQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                return Result<UserDto>.Failure("Usuário não encontrado");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                LastLoginAt = user.LastLoginAt,
                Role = user.Role,
                EmailConfirmed = user.EmailConfirmed
            };

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Erro ao buscar usuário: {ex.Message}");
        }
    }
}
