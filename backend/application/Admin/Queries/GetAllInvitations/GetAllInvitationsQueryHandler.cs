using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Admin.Queries.GetAllInvitations;

public class GetAllInvitationsQueryHandler : IRequestHandler<GetAllInvitationsQuery, Result<List<InvitationTokenDto>>>
{
    private readonly IRepository<InvitationToken> _invitationRepository;

    public GetAllInvitationsQueryHandler(IRepository<InvitationToken> invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<Result<List<InvitationTokenDto>>> Handle(GetAllInvitationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var invitations = await _invitationRepository.GetQueryable()
                .Include(i => i.CreatedByUser)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(cancellationToken);

            var invitationDtos = invitations.Select(i => new InvitationTokenDto
            {
                Id = i.Id,
                Token = i.Token,
                Email = i.Email,
                Role = i.Role,
                RoleName = GetRoleName(i.Role),
                ExpiresAt = i.ExpiresAt,
                IsUsed = i.IsUsed,
                CreatedByUserName = i.CreatedByUser != null ? i.CreatedByUser.FullName : null,
                CreatedAt = i.CreatedAt
            }).ToList();

            return Result<List<InvitationTokenDto>>.Success(invitationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<InvitationTokenDto>>.Failure($"Erro ao buscar convites: {ex.Message}");
        }
    }

    private string GetRoleName(Domain.Enums.UserRole role)
    {
        return role switch
        {
            Domain.Enums.UserRole.Paciente => "Paciente",
            Domain.Enums.UserRole.Profissional => "Profissional",
            Domain.Enums.UserRole.Administrador => "Administrador",
            _ => "Desconhecido"
        };
    }
}
