using Application.DTOs.BloqueiosAgenda;

namespace Application.Interfaces;

/// <summary>
/// Serviço de bloqueio de agenda
/// </summary>
public interface IBloqueioAgendaService
{
    Task<BloqueiosAgendaPaginadosDto> ObterBloqueiosAsync(int pagina, int tamanhoPagina, Guid? profissionalId, string? status);
    Task<BloqueioAgendaDto?> ObterBloqueioPorIdAsync(Guid id);
    Task<List<BloqueioAgendaDto>> ObterBloqueiosPorProfissionalAsync(Guid profissionalId, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<BloqueioAgendaDto> CriarBloqueioAsync(Guid profissionalId, CriarBloqueioAgendaDto dto);
    Task<BloqueioAgendaDto?> AtualizarBloqueioAsync(Guid id, AtualizarBloqueioAgendaDto dto);
    Task<bool> DeletarBloqueioAsync(Guid id);
    Task<bool> AprovarBloqueioAsync(Guid id, Guid aprovadorId);
    Task<bool> RejeitarBloqueioAsync(Guid id, Guid aprovadorId, string motivo);
}
