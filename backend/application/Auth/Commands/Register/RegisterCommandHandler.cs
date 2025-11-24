using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<EmailConfirmationToken> _confirmationTokenRepository;
    private readonly IRepository<InvitationToken> _invitationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IRepository<User> userRepository,
        IRepository<EmailConfirmationToken> confirmationTokenRepository,
        IRepository<InvitationToken> invitationRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _confirmationTokenRepository = confirmationTokenRepository;
        _invitationRepository = invitationRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Se houver token de convite, validar
            InvitationToken? invitation = null;
            if (!string.IsNullOrEmpty(request.InvitationToken))
            {
                Console.WriteLine($"[REGISTER] Token de convite recebido: {request.InvitationToken}");
                var invitations = await _invitationRepository.GetAllAsync();
                invitation = invitations.FirstOrDefault(i => i.Token == request.InvitationToken);

                if (invitation == null)
                {
                    Console.WriteLine($"[REGISTER] Convite não encontrado para token: {request.InvitationToken}");
                    return Result<UserDto>.Failure("Convite não encontrado");
                }

                Console.WriteLine($"[REGISTER] Convite encontrado - ID: {invitation.Id}, IsUsed: {invitation.IsUsed}");

                if (invitation.IsUsed)
                {
                    return Result<UserDto>.Failure("Este convite já foi utilizado");
                }

                if (invitation.ExpiresAt <= DateTime.UtcNow)
                {
                    return Result<UserDto>.Failure("Este convite expirou");
                }
            }
            else
            {
                Console.WriteLine("[REGISTER] Nenhum token de convite fornecido");
            }

            // Verificar se email já existe
            var users = await _userRepository.GetAllAsync();
            if (users.Any(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return Result<UserDto>.Failure("Email já está em uso");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                EmailConfirmed = false
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Marcar convite como usado, se houver
            if (invitation != null)
            {
                Console.WriteLine($"[REGISTER] Marcando convite {invitation.Id} como usado");
                invitation.IsUsed = true;
                await _invitationRepository.UpdateAsync(invitation);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                Console.WriteLine($"[REGISTER] Convite {invitation.Id} marcado como usado com sucesso");
            }

            // Gerar token de confirmação
            var confirmationToken = new EmailConfirmationToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };

            await _confirmationTokenRepository.AddAsync(confirmationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Enviar email de confirmação
            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationToken.Token);

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed
            };

            return Result<UserDto>.Success(userDto, "Cadastro realizado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Erro ao realizar cadastro: {ex.Message}");
        }
    }
}
