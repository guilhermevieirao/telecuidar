using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Adicionar validação de email único
            // TODO: Hash da senha com bcrypt ou similar

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = request.Password // TODO: Hash real
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                LastLoginAt = user.LastLoginAt
            };

            return Result<UserDto>.Success(userDto, "Usuário criado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Erro ao criar usuário: {ex.Message}");
        }
    }
}
