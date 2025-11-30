using MediatR;
using app.Application.Common.Exceptions;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IRepository<User> userRepository, 
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            
            if (user == null)
            {
                return Result<string>.Failure("Usuário não encontrado");
            }

            // Verificar se a senha atual está correta
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return Result<string>.Failure("Senha atual incorreta");
            }

            // Validar nova senha
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            {
                return Result<string>.Failure("A nova senha deve ter no mínimo 6 caracteres");
            }

            // Não permitir a mesma senha
            if (_passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash))
            {
                return Result<string>.Failure("A nova senha não pode ser igual à senha anterior");
            }

            // Atualizar a senha
            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Senha alterada com sucesso", "Senha alterada com sucesso");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Erro ao alterar senha: {ex.Message}");
        }
    }
}
