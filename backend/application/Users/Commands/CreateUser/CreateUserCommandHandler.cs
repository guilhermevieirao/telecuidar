using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IRepository<User> userRepository, 
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar email único
            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return Result<UserDto>.Failure("Já existe um usuário com este email");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = request.Role
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
                LastLoginAt = user.LastLoginAt,
                Role = user.Role
            };

            return Result<UserDto>.Success(userDto, "Usuário criado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Erro ao criar usuário: {ex.Message}");
        }
    }
}
