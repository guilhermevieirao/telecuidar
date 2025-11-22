using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    private readonly IRepository<EmailConfirmationToken> _tokenRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailCommandHandler(
        IRepository<EmailConfirmationToken> tokenRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await _tokenRepository.GetAllAsync();
            var token = tokens.FirstOrDefault(t => t.Token == request.Token && !t.IsUsed);

            if (token == null)
            {
                return Result<bool>.Failure("Token de confirmação inválido ou já utilizado");
            }

            if (token.ExpiresAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Token de confirmação expirado");
            }

            var user = await _userRepository.GetByIdAsync(token.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("Usuário não encontrado");
            }

            user.EmailConfirmed = true;
            token.IsUsed = true;

            await _userRepository.UpdateAsync(user);
            await _tokenRepository.UpdateAsync(token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Email confirmado com sucesso!");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao confirmar email: {ex.Message}");
        }
    }
}
