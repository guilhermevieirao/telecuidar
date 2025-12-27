using Application.DTOs.Agendas;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de agendas
/// </summary>
[ApiController]
[Route("api/agendas")]
[Authorize]
public class AgendasController : ControllerBase
{
    private readonly IAgendaService _agendaService;

    public AgendasController(IAgendaService agendaService)
    {
        _agendaService = agendaService;
    }

    /// <summary>
    /// Listar agendas
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<AgendasPaginadasDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] Guid? profissionalId = null,
        [FromQuery] string? status = null)
    {
        var resultado = await _agendaService.ObterAgendasAsync(pagina, tamanhoPagina, profissionalId, status);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter agenda por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AgendaDto>> ObterPorId(Guid id)
    {
        var agenda = await _agendaService.ObterAgendaPorIdAsync(id);
        if (agenda == null)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return Ok(agenda);
    }

    /// <summary>
    /// Obter agenda do profissional atual
    /// </summary>
    [HttpGet("minha-agenda")]
    [Authorize(Roles = "Profissional")]
    public async Task<ActionResult<AgendaDto>> ObterMinhaAgenda()
    {
        var profissionalId = ObterUsuarioIdAtual();
        var agenda = await _agendaService.ObterAgendaPorProfissionalAsync(profissionalId);
        
        if (agenda == null)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return Ok(agenda);
    }

    /// <summary>
    /// Obter agenda por profissional
    /// </summary>
    [HttpGet("profissional/{profissionalId:guid}")]
    public async Task<ActionResult<AgendaDto>> ObterPorProfissional(Guid profissionalId)
    {
        var agenda = await _agendaService.ObterAgendaPorProfissionalAsync(profissionalId);
        if (agenda == null)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return Ok(agenda);
    }

    /// <summary>
    /// Criar agenda
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AgendaDto>> Criar([FromBody] CriarAgendaDto dto)
    {
        try
        {
            // Se for profissional, usar seu próprio ID
            if (User.IsInRole("Profissional"))
            {
                dto.ProfissionalId = ObterUsuarioIdAtual();
            }

            var agenda = await _agendaService.CriarAgendaAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = agenda.Id }, agenda);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar agenda
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AgendaDto>> Atualizar(Guid id, [FromBody] AtualizarAgendaDto dto)
    {
        try
        {
            var agenda = await _agendaService.AtualizarAgendaAsync(id, dto);
            if (agenda == null)
            {
                return NotFound(new { mensagem = "Agenda não encontrada." });
            }

            return Ok(agenda);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar agenda
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _agendaService.DeletarAgendaAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return NoContent();
    }

    /// <summary>
    /// Ativar agenda
    /// </summary>
    [HttpPost("{id:guid}/ativar")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var sucesso = await _agendaService.AtivarAgendaAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return Ok(new { mensagem = "Agenda ativada com sucesso." });
    }

    /// <summary>
    /// Desativar agenda
    /// </summary>
    [HttpPost("{id:guid}/desativar")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var sucesso = await _agendaService.DesativarAgendaAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Agenda não encontrada." });
        }

        return Ok(new { mensagem = "Agenda desativada com sucesso." });
    }

    /// <summary>
    /// Verificar disponibilidade
    /// </summary>
    [HttpGet("disponibilidade")]
    public async Task<ActionResult<bool>> VerificarDisponibilidade(
        [FromQuery] Guid profissionalId,
        [FromQuery] string data,
        [FromQuery] string horario)
    {
        if (!DateTime.TryParse(data, out var dataConsulta))
        {
            return BadRequest(new { mensagem = "Data inválida." });
        }

        var disponivel = await _agendaService.ProfissionalDisponivelAsync(profissionalId, dataConsulta, horario);
        return Ok(new { disponivel });
    }

    /// <summary>
    /// Obter dias disponíveis no mês
    /// </summary>
    [HttpGet("dias-disponiveis")]
    public async Task<ActionResult<List<DateTime>>> ObterDiasDisponiveis(
        [FromQuery] Guid profissionalId,
        [FromQuery] int ano,
        [FromQuery] int mes)
    {
        var dias = await _agendaService.ObterDiasDisponiveisNoMesAsync(profissionalId, ano, mes);
        return Ok(dias);
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
