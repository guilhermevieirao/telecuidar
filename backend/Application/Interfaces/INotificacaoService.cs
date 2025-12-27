using Application.DTOs.Notificacoes;

namespace Application.Interfaces;

/// <summary>
/// Serviço de notificações
/// </summary>
public interface INotificacaoService
{
    Task<NotificacoesPaginadasDto> ObterNotificacoesAsync(Guid usuarioId, int pagina, int tamanhoPagina, bool? apenasNaoLidas = null);
    Task<NotificacaoDto?> ObterNotificacaoPorIdAsync(Guid id);
    Task<int> ContarNaoLidasAsync(Guid usuarioId);
    Task<NotificacaoDto> CriarNotificacaoAsync(CriarNotificacaoDto dto);
    Task<bool> MarcarComoLidaAsync(Guid id);
    Task<bool> MarcarTodasComoLidasAsync(Guid usuarioId);
    Task<bool> DeletarNotificacaoAsync(Guid id);
    Task<bool> DeletarTodasAsync(Guid usuarioId);
    
    // Notificações específicas
    Task EnviarNotificacaoNovaConsultaAsync(Guid pacienteId, Guid profissionalId, DateTime dataConsulta, string horario);
    Task EnviarNotificacaoConsultaCanceladaAsync(Guid pacienteId, Guid profissionalId, DateTime dataConsulta, string horario);
    Task EnviarNotificacaoLembreteConsultaAsync(Guid usuarioId, DateTime dataConsulta, string horario);
}
