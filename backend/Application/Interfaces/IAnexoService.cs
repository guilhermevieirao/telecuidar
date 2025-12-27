using Application.DTOs.Anexos;

namespace Application.Interfaces;

/// <summary>
/// Serviço de anexos (arquivos de consulta e chat)
/// </summary>
public interface IAnexoService
{
    // Anexos de consulta
    Task<AnexoDto?> ObterAnexoPorIdAsync(Guid id);
    Task<List<AnexoDto>> ObterAnexosPorConsultaAsync(Guid consultaId);
    Task<AnexoDto> SalvarAnexoAsync(Guid consultaId, Guid usuarioId, Stream arquivo, string nomeOriginal, string tipoMime);
    Task<bool> DeletarAnexoAsync(Guid id, Guid usuarioId);
    Task<(Stream Arquivo, string TipoMime, string NomeOriginal)?> BaixarAnexoAsync(Guid id);

    // Anexos de chat
    Task<AnexoChatDto?> ObterAnexoChatPorIdAsync(Guid id);
    Task<List<AnexoChatDto>> ObterAnexosChatPorConsultaAsync(Guid consultaId);
    Task<AnexoChatDto> SalvarAnexoChatAsync(Guid consultaId, Guid usuarioId, Stream arquivo, string nomeOriginal, string tipoMime);
    Task<bool> DeletarAnexoChatAsync(Guid id, Guid usuarioId);
    Task<(Stream Arquivo, string TipoMime, string NomeOriginal)?> BaixarAnexoChatAsync(Guid id);
}
