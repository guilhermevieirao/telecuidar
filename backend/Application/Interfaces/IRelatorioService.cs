using Application.DTOs.Relatorios;

namespace Application.Interfaces;

/// <summary>
/// Serviço de geração de relatórios
/// </summary>
public interface IRelatorioService
{
    /// <summary>
    /// Gera relatório do dashboard
    /// </summary>
    Task<RelatorioDashboardDto> GerarRelatorioDashboardAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

    /// <summary>
    /// Gera relatório de consultas
    /// </summary>
    Task<RelatorioConsultasDto> GerarRelatorioConsultasAsync(FiltrosRelatorioDto filtros);

    /// <summary>
    /// Gera relatório de usuários
    /// </summary>
    Task<RelatorioUsuariosDto> GerarRelatorioUsuariosAsync(FiltrosRelatorioDto filtros);

    /// <summary>
    /// Gera relatório financeiro
    /// </summary>
    Task<RelatorioFinanceiroDto> GerarRelatorioFinanceiroAsync(FiltrosRelatorioDto filtros);

    /// <summary>
    /// Exporta relatório em diferentes formatos
    /// </summary>
    Task<byte[]> ExportarRelatorioAsync(string tipoRelatorio, string formato, FiltrosRelatorioDto filtros);
}
