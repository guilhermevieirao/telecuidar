using Application.DTOs.Agendas;

namespace Application.Interfaces;

/// <summary>
/// Serviço de agenda
/// </summary>
public interface IAgendaService
{
    Task<AgendasPaginadasDto> ObterAgendasAsync(int pagina, int tamanhoPagina, Guid? profissionalId, string? status);
    Task<AgendaDto?> ObterAgendaPorIdAsync(Guid id);
    Task<AgendaDto?> ObterAgendaPorProfissionalAsync(Guid profissionalId);
    Task<AgendaDto> CriarAgendaAsync(CriarAgendaDto dto);
    Task<AgendaDto?> AtualizarAgendaAsync(Guid id, AtualizarAgendaDto dto);
    Task<bool> DeletarAgendaAsync(Guid id);
    Task<bool> AtivarAgendaAsync(Guid id);
    Task<bool> DesativarAgendaAsync(Guid id);
    
    // Verificações
    Task<bool> ProfissionalDisponivelAsync(Guid profissionalId, DateTime data, string horario);
    Task<List<DateTime>> ObterDiasDisponiveisNoMesAsync(Guid profissionalId, int ano, int mes);
}
