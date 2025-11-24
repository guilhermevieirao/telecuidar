using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Enums;
using app.Domain.Interfaces;

namespace app.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result<LoginResponseDto>.Failure("Email ou senha inválidos");
            }

            if (!user.IsActive)
            {
                return Result<LoginResponseDto>.Failure("Usuário inativo");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    AvatarUrl = user.AvatarUrl,
                    LastLoginAt = user.LastLoginAt
                }
            };

            return Result<LoginResponseDto>.Success(response, "Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure($"Erro ao realizar login: {ex.Message}");
        }
    }
}
