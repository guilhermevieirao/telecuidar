using MediatR;
using app.Application.Common.Exceptions;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _userRepository.ExistsAsync(request.Id);
            
            if (!exists)
            {
                throw new NotFoundException(nameof(User), request.Id);
            }

            var deleted = await _userRepository.DeleteAsync(request.Id);
            
            if (deleted)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true, "Usuário deletado com sucesso");
            }

            return Result<bool>.Failure("Falha ao deletar usuário");
        }
        catch (NotFoundException)
        {
            return Result<bool>.Failure("Usuário não encontrado");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao deletar usuário: {ex.Message}");
        }
    }
}
