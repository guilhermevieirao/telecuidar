using Application.DTOs.Atestados;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de atestados médicos
/// </summary>
[ApiController]
[Route("api/atestados")]
[Authorize]
public class AtestadosController : ControllerBase
{
    private readonly IAtestadoMedicoService _atestadoService;
    private readonly ILogAuditoriaService _logAuditoriaService;

    public AtestadosController(IAtestadoMedicoService atestadoService, ILogAuditoriaService logAuditoriaService)
    {
        _atestadoService = atestadoService;
        _logAuditoriaService = logAuditoriaService;
    }

    /// <summary>
    /// Listar atestados
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AtestadosPaginadosDto>> Listar([FromQuery] FiltrosAtestadoDto filtros)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        // Pacientes só veem seus próprios atestados
        if (tipo == "Paciente")
        {
            filtros.PacienteId = usuarioId;
        }
        else if (tipo == "Profissional")
        {
            filtros.ProfissionalId = usuarioId;
        }

        var resultado = await _atestadoService.ObterAtestadosAsync(filtros);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter atestado por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AtestadoDto>> ObterPorId(Guid id)
    {
        var atestado = await _atestadoService.ObterAtestadoPorIdAsync(id);
        if (atestado == null)
        {
            return NotFound(new { mensagem = "Atestado não encontrado." });
        }

        return Ok(atestado);
    }

    /// <summary>
    /// Obter atestados por consulta
    /// </summary>
    [HttpGet("consulta/{consultaId:guid}")]
    public async Task<ActionResult<List<AtestadoDto>>> ObterPorConsulta(Guid consultaId)
    {
        var atestados = await _atestadoService.ObterAtestadosPorConsultaAsync(consultaId);
        return Ok(atestados);
    }

    /// <summary>
    /// Criar atestado
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AtestadoDto>> Criar([FromBody] CriarAtestadoDto dto)
    {
        try
        {
            var atestado = await _atestadoService.CriarAtestadoAsync(dto);

            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "criar",
                "AtestadoMedico",
                atestado.Id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return CreatedAtAction(nameof(ObterPorId), new { id = atestado.Id }, atestado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar atestado
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AtestadoDto>> Atualizar(Guid id, [FromBody] AtualizarAtestadoDto dto)
    {
        try
        {
            var atestado = await _atestadoService.AtualizarAtestadoAsync(id, dto);
            if (atestado == null)
            {
                return NotFound(new { mensagem = "Atestado não encontrado." });
            }

            return Ok(atestado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar atestado
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        try
        {
            var sucesso = await _atestadoService.DeletarAtestadoAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Atestado não encontrado." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Assinar atestado digitalmente
    /// </summary>
    [HttpPost("{id:guid}/assinar")]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> Assinar(Guid id, [FromBody] AssinarAtestadoDto dto)
    {
        try
        {
            var profissionalId = ObterUsuarioIdAtual();
            var sucesso = await _atestadoService.AssinarAtestadoAsync(id, profissionalId, dto.CertificadoId);
            
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Atestado não encontrado." });
            }

            await _logAuditoriaService.RegistrarAsync(
                profissionalId,
                "assinar",
                "AtestadoMedico",
                id,
                null,
                new { dto.CertificadoId },
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(new { mensagem = "Atestado assinado com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Gerar PDF do atestado
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GerarPdf(Guid id)
    {
        try
        {
            var pdf = await _atestadoService.GerarPdfAtestadoAsync(id);
            return File(pdf, "application/pdf", $"atestado_{id}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string ObterTipoUsuario()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Paciente";
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
/// DTO para assinar atestado
/// </summary>
public class AssinarAtestadoDto
{
    public string CertificadoId { get; set; } = string.Empty;
}
