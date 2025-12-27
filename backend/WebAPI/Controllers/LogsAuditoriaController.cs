using Application.DTOs.LogsAuditoria;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de logs de auditoria
/// </summary>
[ApiController]
[Route("api/logs-auditoria")]
[Authorize(Roles = "Administrador")]
public class LogsAuditoriaController : ControllerBase
{
    private readonly ILogAuditoriaService _logAuditoriaService;

    public LogsAuditoriaController(ILogAuditoriaService logAuditoriaService)
    {
        _logAuditoriaService = logAuditoriaService;
    }

    /// <summary>
    /// Listar logs de auditoria
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<LogsAuditoriaPaginadosDto>> Listar([FromQuery] FiltrosLogAuditoriaDto filtros)
    {
        var resultado = await _logAuditoriaService.ObterLogsAsync(filtros);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter log por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LogAuditoriaDto>> ObterPorId(Guid id)
    {
        var log = await _logAuditoriaService.ObterLogPorIdAsync(id);
        if (log == null)
        {
            return NotFound(new { mensagem = "Log não encontrado." });
        }

        return Ok(log);
    }

    /// <summary>
    /// Limpar logs antigos
    /// </summary>
    [HttpDelete("limpar")]
    public async Task<IActionResult> LimparAntigos([FromQuery] int diasRetencao = 90)
    {
        if (diasRetencao < 30)
        {
            return BadRequest(new { mensagem = "O período de retenção mínimo é de 30 dias." });
        }

        var sucesso = await _logAuditoriaService.DeletarLogsAntigosAsync(diasRetencao);
        if (!sucesso)
        {
            return StatusCode(500, new { mensagem = "Erro ao limpar logs antigos." });
        }

        return Ok(new { mensagem = $"Logs com mais de {diasRetencao} dias foram removidos." });
    }
}
