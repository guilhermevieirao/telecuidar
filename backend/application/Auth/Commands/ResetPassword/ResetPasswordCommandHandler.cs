using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<PasswordResetToken> _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        IRepository<User> userRepository,
        IRepository<PasswordResetToken> tokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allTokens = await _tokenRepository.GetAllAsync();
            var resetToken = allTokens.FirstOrDefault(t => t.Token == request.Token);

            if (resetToken == null)
            {
                return Result<bool>.Failure("Token inválido");
            }

            if (resetToken.IsUsed)
            {
                return Result<bool>.Failure("Token já utilizado");
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Token expirado");
            }

            var user = await _userRepository.GetByIdAsync(resetToken.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("Usuário não encontrado");
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            resetToken.IsUsed = true;
            await _tokenRepository.UpdateAsync(resetToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Senha redefinida com sucesso");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao redefinir senha: {ex.Message}");
        }
    }
}
