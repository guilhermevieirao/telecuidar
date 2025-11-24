using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Admin.Queries.GetInvitationByToken;

public class GetInvitationByTokenQueryHandler : IRequestHandler<GetInvitationByTokenQuery, Result<InvitationDetailsDto>>
{
    private readonly IRepository<InvitationToken> _invitationRepository;

    public GetInvitationByTokenQueryHandler(IRepository<InvitationToken> invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<Result<InvitationDetailsDto>> Handle(GetInvitationByTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var invitations = await _invitationRepository.GetAllAsync();
            var invitation = invitations.FirstOrDefault(i => i.Token == request.Token);

            if (invitation == null)
            {
                return Result<InvitationDetailsDto>.Failure("Convite não encontrado");
            }

            if (invitation.IsUsed)
            {
                return Result<InvitationDetailsDto>.Failure("Este convite já foi utilizado");
            }

            var isExpired = invitation.ExpiresAt <= DateTime.UtcNow;

            var dto = new InvitationDetailsDto
            {
                Token = invitation.Token,
                Email = invitation.Email,
                Role = invitation.Role,
                FirstName = invitation.FirstName,
                LastName = invitation.LastName,
                PhoneNumber = invitation.PhoneNumber,
                ExpiresAt = invitation.ExpiresAt,
                IsExpired = isExpired
            };

            if (isExpired)
            {
                return Result<InvitationDetailsDto>.Failure("Este convite expirou");
            }

            return Result<InvitationDetailsDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<InvitationDetailsDto>.Failure($"Erro ao buscar convite: {ex.Message}");
        }
    }
}
