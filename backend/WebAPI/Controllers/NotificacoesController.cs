using Application.DTOs.Notificacoes;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de notificações
/// </summary>
[ApiController]
[Route("api/notificacoes")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacoesController(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    /// <summary>
    /// Listar minhas notificações
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<NotificacoesPaginadasDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        [FromQuery] bool? apenasNaoLidas = null)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var resultado = await _notificacaoService.ObterNotificacoesAsync(usuarioId, pagina, tamanhoPagina, apenasNaoLidas);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter contagem de não lidas
    /// </summary>
    [HttpGet("nao-lidas/contagem")]
    public async Task<ActionResult<int>> ContarNaoLidas()
    {
        var usuarioId = ObterUsuarioIdAtual();
        var contagem = await _notificacaoService.ContarNaoLidasAsync(usuarioId);
        return Ok(new { contagem });
    }

    /// <summary>
    /// Marcar como lida
    /// </summary>
    [HttpPost("{id:guid}/marcar-lida")]
    public async Task<IActionResult> MarcarComoLida(Guid id)
    {
        var sucesso = await _notificacaoService.MarcarComoLidaAsync(id);
        
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Notificação não encontrada." });
        }

        return Ok(new { mensagem = "Notificação marcada como lida." });
    }

    /// <summary>
    /// Marcar todas como lidas
    /// </summary>
    [HttpPost("marcar-todas-lidas")]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        var usuarioId = ObterUsuarioIdAtual();
        await _notificacaoService.MarcarTodasComoLidasAsync(usuarioId);
        return Ok(new { mensagem = "Todas as notificações foram marcadas como lidas." });
    }

    /// <summary>
    /// Deletar notificação
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _notificacaoService.DeletarNotificacaoAsync(id);
        
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Notificação não encontrada." });
        }

        return NoContent();
    }

    /// <summary>
    /// Enviar notificação (admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Enviar([FromBody] CriarNotificacaoDto dto)
    {
        await _notificacaoService.CriarNotificacaoAsync(dto);

        return Ok(new { mensagem = "Notificação enviada com sucesso." });
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
