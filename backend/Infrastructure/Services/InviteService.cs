using Application.DTOs.Invites;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class InviteService : IInviteService
{
    private readonly ApplicationDbContext _context;

    public InviteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedInvitesDto> GetInvitesAsync(
        int page, 
        int pageSize, 
        string? sortBy, 
        string? sortDirection, 
        string? role, 
        string? status)
    {
        // TODO: Implementar quando a entidade Invite for criada no banco
        // Por enquanto retorna lista vazia para não dar erro 404
        await Task.CompletedTask;

        return new PaginatedInvitesDto
        {
            Data = new List<InviteDto>(),
            Total = 0,
            Page = page,
            PageSize = pageSize,
            TotalPages = 0
        };
    }

    public async Task<InviteDto?> GetInviteByIdAsync(Guid id)
    {
        // TODO: Implementar quando a entidade Invite for criada no banco
        await Task.CompletedTask;
        return null;
    }

    public async Task<InviteDto> CreateInviteAsync(CreateInviteDto dto)
    {
        // TODO: Implementar quando a entidade Invite for criada no banco
        // Por enquanto retorna um objeto mock
        await Task.CompletedTask;

        return new InviteDto
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Role = dto.Role,
            Status = "PENDING",
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.Empty,
            CreatedByName = "System",
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<bool> DeleteInviteAsync(Guid id)
    {
        // TODO: Implementar quando a entidade Invite for criada no banco
        await Task.CompletedTask;
        return false;
    }
}
