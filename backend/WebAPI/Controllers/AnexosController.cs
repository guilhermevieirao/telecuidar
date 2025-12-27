using Application.DTOs.Anexos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de anexos
/// </summary>
[ApiController]
[Route("api/anexos")]
[Authorize]
public class AnexosController : ControllerBase
{
    private readonly IAnexoService _anexoService;

    public AnexosController(IAnexoService anexoService)
    {
        _anexoService = anexoService;
    }

    /// <summary>
    /// Obter anexo por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AnexoDto>> ObterPorId(Guid id)
    {
        var anexo = await _anexoService.ObterAnexoPorIdAsync(id);
        if (anexo == null)
        {
            return NotFound(new { mensagem = "Anexo não encontrado." });
        }

        return Ok(anexo);
    }

    /// <summary>
    /// Listar anexos por consulta
    /// </summary>
    [HttpGet("consulta/{consultaId:guid}")]
    public async Task<ActionResult<List<AnexoDto>>> ListarPorConsulta(Guid consultaId)
    {
        var anexos = await _anexoService.ObterAnexosPorConsultaAsync(consultaId);
        return Ok(anexos);
    }

    /// <summary>
    /// Fazer upload de anexo
    /// </summary>
    [HttpPost("consulta/{consultaId:guid}")]
    public async Task<ActionResult<AnexoDto>> Upload(Guid consultaId, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            return BadRequest(new { mensagem = "Arquivo não fornecido." });
        }

        // Limite de 10MB
        if (arquivo.Length > 10 * 1024 * 1024)
        {
            return BadRequest(new { mensagem = "Arquivo muito grande. Limite de 10MB." });
        }

        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            using var stream = arquivo.OpenReadStream();
            var anexo = await _anexoService.SalvarAnexoAsync(consultaId, usuarioId, stream, arquivo.FileName, arquivo.ContentType);
            return CreatedAtAction(nameof(ObterPorId), new { id = anexo.Id }, anexo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Download de anexo
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var resultado = await _anexoService.BaixarAnexoAsync(id);
        if (resultado == null)
        {
            return NotFound(new { mensagem = "Anexo não encontrado." });
        }

        return File(resultado.Value.Arquivo, resultado.Value.TipoMime, resultado.Value.NomeOriginal);
    }

    /// <summary>
    /// Deletar anexo
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            var sucesso = await _anexoService.DeletarAnexoAsync(id, usuarioId);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Anexo não encontrado." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // ========== ANEXOS DE CHAT ==========

    /// <summary>
    /// Obter anexo de chat por ID
    /// </summary>
    [HttpGet("chat/{id:guid}")]
    public async Task<ActionResult<AnexoChatDto>> ObterChatPorId(Guid id)
    {
        var anexo = await _anexoService.ObterAnexoChatPorIdAsync(id);
        if (anexo == null)
        {
            return NotFound(new { mensagem = "Anexo não encontrado." });
        }

        return Ok(anexo);
    }

    /// <summary>
    /// Listar anexos de chat por consulta
    /// </summary>
    [HttpGet("chat/consulta/{consultaId:guid}")]
    public async Task<ActionResult<List<AnexoChatDto>>> ListarChatPorConsulta(Guid consultaId)
    {
        var anexos = await _anexoService.ObterAnexosChatPorConsultaAsync(consultaId);
        return Ok(anexos);
    }

    /// <summary>
    /// Upload de anexo de chat
    /// </summary>
    [HttpPost("chat/consulta/{consultaId:guid}")]
    public async Task<ActionResult<AnexoChatDto>> UploadChat(Guid consultaId, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            return BadRequest(new { mensagem = "Arquivo não fornecido." });
        }

        // Limite de 5MB para chat
        if (arquivo.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { mensagem = "Arquivo muito grande. Limite de 5MB para chat." });
        }

        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            using var stream = arquivo.OpenReadStream();
            var anexo = await _anexoService.SalvarAnexoChatAsync(consultaId, usuarioId, stream, arquivo.FileName, arquivo.ContentType);
            return CreatedAtAction(nameof(ObterChatPorId), new { id = anexo.Id }, anexo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Download de anexo de chat
    /// </summary>
    [HttpGet("chat/{id:guid}/download")]
    public async Task<IActionResult> DownloadChat(Guid id)
    {
        var resultado = await _anexoService.BaixarAnexoChatAsync(id);
        if (resultado == null)
        {
            return NotFound(new { mensagem = "Anexo não encontrado." });
        }

        return File(resultado.Value.Arquivo, resultado.Value.TipoMime, resultado.Value.NomeOriginal);
    }

    /// <summary>
    /// Deletar anexo de chat
    /// </summary>
    [HttpDelete("chat/{id:guid}")]
    public async Task<IActionResult> DeletarChat(Guid id)
    {
        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            var sucesso = await _anexoService.DeletarAnexoChatAsync(id, usuarioId);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Anexo não encontrado." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
