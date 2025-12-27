using Application.DTOs.Convites;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de convites
/// </summary>
[ApiController]
[Route("api/convites")]
[Authorize]
public class ConvitesController : ControllerBase
{
    private readonly IConviteService _conviteService;
    private readonly ILogAuditoriaService _logAuditoriaService;

    public ConvitesController(IConviteService conviteService, ILogAuditoriaService logAuditoriaService)
    {
        _conviteService = conviteService;
        _logAuditoriaService = logAuditoriaService;
    }

    /// <summary>
    /// Listar convites
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ConvitesPaginadosDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? status = null)
    {
        var resultado = await _conviteService.ObterConvitesAsync(pagina, tamanhoPagina, status);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter convite por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ConviteDto>> ObterPorId(Guid id)
    {
        var convite = await _conviteService.ObterConvitePorIdAsync(id);
        if (convite == null)
        {
            return NotFound(new { mensagem = "Convite não encontrado." });
        }

        return Ok(convite);
    }

    /// <summary>
    /// Validar token de convite (público)
    /// </summary>
    [HttpGet("validar/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<ValidarConviteResponseDto>> ValidarToken(string token)
    {
        var resultado = await _conviteService.ValidarTokenAsync(token);
        if (!resultado.Valido)
        {
            return NotFound(new { mensagem = "Convite inválido ou expirado." });
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Criar convite
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ConviteDto>> Criar([FromBody] CriarConviteDto dto)
    {
        try
        {
            var criadoPorId = ObterUsuarioIdAtual();
            var convite = await _conviteService.CriarConviteAsync(dto, criadoPorId);

            await _logAuditoriaService.RegistrarAsync(
                criadoPorId,
                "criar",
                "Convite",
                convite.Id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return CreatedAtAction(nameof(ObterPorId), new { id = convite.Id }, convite);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Usar convite (registrar com convite)
    /// </summary>
    [HttpPost("usar/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> Usar(string token, [FromBody] UsarConviteDto dto)
    {
        try
        {
            var sucesso = await _conviteService.AceitarConviteAsync(token, dto.UsuarioId);
            if (!sucesso)
            {
                return BadRequest(new { mensagem = "Não foi possível usar o convite." });
            }

            return Ok(new { mensagem = "Convite usado com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar convite
    /// </summary>
    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var sucesso = await _conviteService.RevogarConviteAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Convite não encontrado." });
        }

        await _logAuditoriaService.RegistrarAsync(
            ObterUsuarioIdAtual(),
            "cancelar",
            "Convite",
            id,
            null,
            null,
            ObterEnderecoIp(),
            ObterUserAgent()
        );

        return Ok(new { mensagem = "Convite cancelado com sucesso." });
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string? ObterEnderecoIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? ObterUserAgent()
    {
        return HttpContext.Request.Headers.UserAgent.FirstOrDefault();
    }
}

/// <summary>
/// DTO para usar convite
/// </summary>
public class UsarConviteDto
{
    public Guid UsuarioId { get; set; }
}
