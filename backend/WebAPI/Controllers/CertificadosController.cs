using Application.DTOs.Certificados;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de certificados digitais
/// </summary>
[ApiController]
[Route("api/certificados")]
[Authorize(Roles = "Profissional,Administrador")]
public class CertificadosController : ControllerBase
{
    private readonly ICertificadoService _certificadoService;

    public CertificadosController(ICertificadoService certificadoService)
    {
        _certificadoService = certificadoService;
    }

    /// <summary>
    /// Obter certificado por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CertificadoDto>> ObterPorId(Guid id)
    {
        var certificado = await _certificadoService.ObterCertificadoPorIdAsync(id);
        if (certificado == null)
        {
            return NotFound(new { mensagem = "Certificado não encontrado." });
        }

        return Ok(certificado);
    }

    /// <summary>
    /// Listar meus certificados
    /// </summary>
    [HttpGet("meus-certificados")]
    public async Task<ActionResult<List<CertificadoDto>>> MeusCertificados()
    {
        var usuarioId = ObterUsuarioIdAtual();
        var certificados = await _certificadoService.ObterCertificadosPorUsuarioAsync(usuarioId);
        return Ok(certificados);
    }

    /// <summary>
    /// Fazer upload de certificado
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CertificadoDto>> Upload(
        IFormFile arquivo,
        [FromForm] string senha,
        [FromForm] string? descricao)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            return BadRequest(new { mensagem = "Arquivo não fornecido." });
        }

        // Verificar extensão
        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        if (extensao != ".pfx" && extensao != ".p12")
        {
            return BadRequest(new { mensagem = "Formato inválido. Use arquivos .pfx ou .p12." });
        }

        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            using var stream = arquivo.OpenReadStream();
            var apelido = descricao ?? "Certificado Digital";

            var certificado = await _certificadoService.SalvarCertificadoAsync(usuarioId, stream, senha, apelido);
            return CreatedAtAction(nameof(ObterPorId), new { id = certificado.Id }, certificado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Validar certificado
    /// </summary>
    [HttpPost("{id:guid}/validar")]
    public async Task<ActionResult> Validar(Guid id, [FromBody] ValidarCertificadoRequestDto request)
    {
        try
        {
            var valido = await _certificadoService.ValidarCertificadoAsync(id, request.Senha);
            return Ok(new { valido });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar certificado
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            var sucesso = await _certificadoService.DeletarCertificadoAsync(id, usuarioId);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Certificado não encontrado." });
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

/// <summary>
/// Request para validar certificado
/// </summary>
 public class ValidarCertificadoRequestDto
{
    public string Senha { get; set; } = string.Empty;
}
