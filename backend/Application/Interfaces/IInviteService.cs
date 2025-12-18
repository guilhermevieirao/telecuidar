using Application.DTOs.Invites;

namespace Application.Interfaces;

public interface IInviteService
{
    Task<PaginatedInvitesDto> GetInvitesAsync(int page, int pageSize, string? sortBy, string? sortDirection, string? role, string? status);
    Task<InviteDto?> GetInviteByIdAsync(Guid id);
    Task<InviteDto> CreateInviteAsync(CreateInviteDto dto);
    Task<bool> DeleteInviteAsync(Guid id);
}
