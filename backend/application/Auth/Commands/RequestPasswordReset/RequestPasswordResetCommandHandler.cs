using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Auth.Commands.RequestPasswordReset;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Result<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<PasswordResetToken> _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public RequestPasswordResetCommandHandler(
        IRepository<User> userRepository,
        IRepository<PasswordResetToken> tokenRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

            // Retorna sucesso mesmo se o email não existir (segurança)
            if (user == null)
            {
                return Result<bool>.Success(true, "Se o email existir, você receberá instruções para redefinir sua senha");
            }

            // Invalida tokens anteriores
            var existingTokens = (await _tokenRepository.GetAllAsync())
                .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
            
            foreach (var token in existingTokens)
            {
                token.IsUsed = true;
                await _tokenRepository.UpdateAsync(token);
            }

            // Cria novo token
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                IsUsed = false
            };

            await _tokenRepository.AddAsync(resetToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Envia email com token
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, resetToken.Token);

            return Result<bool>.Success(true, "Se o email existir, você receberá instruções para redefinir sua senha");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao solicitar redefinição de senha: {ex.Message}");
        }
    }
}
