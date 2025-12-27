using Application.DTOs.LogsAuditoria;

namespace Application.Interfaces;

/// <summary>
/// Serviço de logs de auditoria
/// </summary>
public interface ILogAuditoriaService
{
    Task<LogsAuditoriaPaginadosDto> ObterLogsAsync(FiltrosLogAuditoriaDto filtros);
    Task<LogAuditoriaDto?> ObterLogPorIdAsync(Guid id);
    Task RegistrarLogAsync(CriarLogAuditoriaDto dto);
    Task RegistrarAsync(Guid? usuarioId, string acao, string? entidade = null, Guid? entidadeId = null, object? dadosAntigos = null, object? dadosNovos = null, string? enderecoIp = null, string? userAgent = null);
    Task<bool> DeletarLogsAntigosAsync(int diasRetencao);
}
