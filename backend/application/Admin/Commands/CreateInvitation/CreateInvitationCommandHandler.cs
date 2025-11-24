using MediatR;
using app.Application.Common.Models;
using app.Application.Common.Interfaces;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Admin.Commands.CreateInvitation;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<InvitationTokenDto>>
{
    private readonly IRepository<InvitationToken> _invitationRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CreateInvitationCommandHandler(
        IRepository<InvitationToken> invitationRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result<InvitationTokenDto>> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar se email já está em uso
            var users = await _userRepository.GetAllAsync();
            if (users.Any(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return Result<InvitationTokenDto>.Failure("Este email já está cadastrado no sistema");
            }

            // Verificar se já existe convite pendente para este email
            var invitations = await _invitationRepository.GetAllAsync();
            var existingInvitation = invitations.FirstOrDefault(i => 
                i.Email.ToLower() == request.Email.ToLower() && 
                !i.IsUsed && 
                i.ExpiresAt > DateTime.UtcNow);

            if (existingInvitation != null)
            {
                return Result<InvitationTokenDto>.Failure("Já existe um convite pendente para este email");
            }

            var invitation = new InvitationToken
            {
                Token = Guid.NewGuid().ToString("N"),
                Email = request.Email.ToLower(),
                Role = request.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(request.ExpirationHours),
                IsUsed = false,
                CreatedByUserId = request.CreatedByUserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            await _invitationRepository.AddAsync(invitation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Enviar email de convite
            var invitationLink = $"http://localhost:4200/cadastrar?token={invitation.Token}";
            await _emailService.SendInvitationEmailAsync(request.Email, invitation.Role.ToString(), invitationLink);

            var createdByUser = await _userRepository.GetByIdAsync(request.CreatedByUserId);

            var invitationDto = new InvitationTokenDto
            {
                Id = invitation.Id,
                Token = invitation.Token,
                Email = invitation.Email,
                Role = invitation.Role,
                RoleName = invitation.Role.ToString(),
                ExpiresAt = invitation.ExpiresAt,
                IsUsed = invitation.IsUsed,
                CreatedByUserName = createdByUser?.FullName,
                CreatedAt = invitation.CreatedAt
            };

            return Result<InvitationTokenDto>.Success(invitationDto, "Convite criado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<InvitationTokenDto>.Failure($"Erro ao criar convite: {ex.Message}");
        }
    }
}
