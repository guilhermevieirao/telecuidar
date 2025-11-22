using MediatR;
using app.Application.Common.Exceptions;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.Id);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.AvatarUrl = request.AvatarUrl;
            user.ProfilePhotoUrl = request.ProfilePhotoUrl;
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
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                LastLoginAt = user.LastLoginAt
            };

            return Result<UserDto>.Success(userDto, "Usuário atualizado com sucesso");
        }
        catch (NotFoundException)
        {
            return Result<UserDto>.Failure("Usuário não encontrado");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Erro ao atualizar usuário: {ex.Message}");
        }
    }
}
