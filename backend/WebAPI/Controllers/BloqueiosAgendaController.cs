using Application.DTOs.BloqueiosAgenda;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de bloqueios de agenda
/// </summary>
[ApiController]
[Route("api/bloqueios-agenda")]
[Authorize]
public class BloqueiosAgendaController : ControllerBase
{
    private readonly IBloqueioAgendaService _bloqueioAgendaService;

    public BloqueiosAgendaController(IBloqueioAgendaService bloqueioAgendaService)
    {
        _bloqueioAgendaService = bloqueioAgendaService;
    }

    /// <summary>
    /// Listar bloqueios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BloqueiosAgendaPaginadosDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] Guid? profissionalId = null,
        [FromQuery] string? status = null)
    {
        // Profissionais só veem seus próprios bloqueios
        if (User.IsInRole("Profissional"))
        {
            profissionalId = ObterUsuarioIdAtual();
        }

        var resultado = await _bloqueioAgendaService.ObterBloqueiosAsync(pagina, tamanhoPagina, profissionalId, status);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter bloqueio por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BloqueioAgendaDto>> ObterPorId(Guid id)
    {
        var bloqueio = await _bloqueioAgendaService.ObterBloqueioPorIdAsync(id);
        if (bloqueio == null)
        {
            return NotFound(new { mensagem = "Bloqueio não encontrado." });
        }

        return Ok(bloqueio);
    }

    /// <summary>
    /// Obter meus bloqueios
    /// </summary>
    [HttpGet("meus-bloqueios")]
    [Authorize(Roles = "Profissional")]
    public async Task<ActionResult<List<BloqueioAgendaDto>>> ObterMeusBloqueios(
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var profissionalId = ObterUsuarioIdAtual();
        var bloqueios = await _bloqueioAgendaService.ObterBloqueiosPorProfissionalAsync(profissionalId, dataInicio, dataFim);
        return Ok(bloqueios);
    }

    /// <summary>
    /// Criar bloqueio
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Profissional")]
    public async Task<ActionResult<BloqueioAgendaDto>> Criar([FromBody] CriarBloqueioAgendaDto dto)
    {
        try
        {
            var profissionalId = ObterUsuarioIdAtual();
            var bloqueio = await _bloqueioAgendaService.CriarBloqueioAsync(profissionalId, dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = bloqueio.Id }, bloqueio);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar bloqueio
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<BloqueioAgendaDto>> Atualizar(Guid id, [FromBody] AtualizarBloqueioAgendaDto dto)
    {
        try
        {
            var bloqueio = await _bloqueioAgendaService.AtualizarBloqueioAsync(id, dto);
            if (bloqueio == null)
            {
                return NotFound(new { mensagem = "Bloqueio não encontrado." });
            }

            return Ok(bloqueio);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar bloqueio
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _bloqueioAgendaService.DeletarBloqueioAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Bloqueio não encontrado." });
        }

        return NoContent();
    }

    /// <summary>
    /// Aprovar bloqueio
    /// </summary>
    [HttpPost("{id:guid}/aprovar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Aprovar(Guid id)
    {
        var aprovadorId = ObterUsuarioIdAtual();
        var sucesso = await _bloqueioAgendaService.AprovarBloqueioAsync(id, aprovadorId);
        
        if (!sucesso)
        {
            return BadRequest(new { mensagem = "Não foi possível aprovar o bloqueio." });
        }

        return Ok(new { mensagem = "Bloqueio aprovado com sucesso." });
    }

    /// <summary>
    /// Rejeitar bloqueio
    /// </summary>
    [HttpPost("{id:guid}/rejeitar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Rejeitar(Guid id, [FromBody] RejeitarBloqueioDto dto)
    {
        var aprovadorId = ObterUsuarioIdAtual();
        var sucesso = await _bloqueioAgendaService.RejeitarBloqueioAsync(id, aprovadorId, dto.Motivo);
        
        if (!sucesso)
        {
            return BadRequest(new { mensagem = "Não foi possível rejeitar o bloqueio." });
        }

        return Ok(new { mensagem = "Bloqueio rejeitado com sucesso." });
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

/// <summary>
/// DTO para rejeitar bloqueio
/// </summary>
public class RejeitarBloqueioDto
{
    public string Motivo { get; set; } = string.Empty;
}
