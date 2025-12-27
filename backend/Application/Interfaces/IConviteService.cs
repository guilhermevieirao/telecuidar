using Application.DTOs.Convites;

namespace Application.Interfaces;

/// <summary>
/// Serviço de convites
/// </summary>
public interface IConviteService
{
    Task<ConvitesPaginadosDto> ObterConvitesAsync(int pagina, int tamanhoPagina, string? status);
    Task<ConviteDto?> ObterConvitePorIdAsync(Guid id);
    Task<ConviteDto> CriarConviteAsync(CriarConviteDto dto, Guid criadorId);
    Task<bool> RevogarConviteAsync(Guid id);
    Task<ValidarConviteResponseDto> ValidarTokenAsync(string token);
    Task<bool> AceitarConviteAsync(string token, Guid usuarioId);
    Task<ConviteDto?> ObterConvitePorTokenAsync(string token);
}
