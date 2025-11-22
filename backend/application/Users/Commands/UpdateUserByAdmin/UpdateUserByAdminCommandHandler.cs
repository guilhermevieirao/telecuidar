using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Interfaces;
using app.Domain.Enums;

namespace app.Application.Users.Commands.UpdateUserByAdmin;

public class UpdateUserByAdminCommandHandler : IRequestHandler<UpdateUserByAdminCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public UpdateUserByAdminCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<Domain.Entities.User> userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserByAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<UserDto>.Failure("Usuário não encontrado", new List<string> { "User not found" });
            }

            // Atualizar campos se fornecidos
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName;

            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                // Verificar se email já existe para outro usuário
                var emailExists = _userRepository.GetQueryable()
                    .Any(u => u.Email == request.Email && u.Id != request.UserId);
                
                if (emailExists)
                {
                    return Result<UserDto>.Failure("E-mail já está em uso", new List<string> { "Email already exists" });
                }
                
                user.Email = request.Email;
            }

            if (request.PhoneNumber != null)
                user.PhoneNumber = request.PhoneNumber;

            if (request.Role.HasValue)
                user.Role = (UserRole)request.Role.Value;

            if (request.EmailConfirmed.HasValue)
                user.EmailConfirmed = request.EmailConfirmed.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
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

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure("Erro ao atualizar usuário", new List<string> { ex.Message });
        }
    }
}
