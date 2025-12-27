using Application.DTOs.Relatorios;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de relatórios
/// </summary>
[ApiController]
[Route("api/relatorios")]
[Route("api/reports")] // Alias em inglês para compatibilidade com frontend
[Authorize(Roles = "Administrador,Profissional")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Obter relatório do dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<RelatorioDashboardDto>> Dashboard(
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var relatorio = await _relatorioService.GerarRelatorioDashboardAsync(dataInicio, dataFim);
        return Ok(relatorio);
    }

    /// <summary>
    /// Obter relatório de consultas
    /// </summary>
    [HttpGet("consultas")]
    public async Task<ActionResult<RelatorioConsultasDto>> Consultas([FromQuery] FiltrosRelatorioDto filtros)
    {
        var relatorio = await _relatorioService.GerarRelatorioConsultasAsync(filtros);
        return Ok(relatorio);
    }

    /// <summary>
    /// Obter relatório de usuários
    /// </summary>
    [HttpGet("usuarios")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<RelatorioUsuariosDto>> Usuarios([FromQuery] FiltrosRelatorioDto filtros)
    {
        var relatorio = await _relatorioService.GerarRelatorioUsuariosAsync(filtros);
        return Ok(relatorio);
    }

    /// <summary>
    /// Obter relatório financeiro
    /// </summary>
    [HttpGet("financeiro")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<RelatorioFinanceiroDto>> Financeiro([FromQuery] FiltrosRelatorioDto filtros)
    {
        var relatorio = await _relatorioService.GerarRelatorioFinanceiroAsync(filtros);
        return Ok(relatorio);
    }

    /// <summary>
    /// Exportar relatório
    /// </summary>
    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar(
        [FromQuery] string tipo,
        [FromQuery] string formato = "csv",
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null,
        [FromQuery] Guid? profissionalId = null,
        [FromQuery] Guid? especialidadeId = null)
    {
        var filtros = new FiltrosRelatorioDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            ProfissionalId = profissionalId,
            EspecialidadeId = especialidadeId
        };

        var arquivo = await _relatorioService.ExportarRelatorioAsync(tipo, formato, filtros);

        var contentType = formato.ToLower() switch
        {
            "csv" => "text/csv",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "pdf" => "application/pdf",
            _ => "text/plain"
        };

        return File(arquivo, contentType, $"relatorio_{tipo}_{DateTime.UtcNow:yyyyMMdd}.{formato}");
    }
}
