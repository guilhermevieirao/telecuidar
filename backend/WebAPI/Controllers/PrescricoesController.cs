using Application.DTOs.Prescricoes;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de prescrições
/// </summary>
[ApiController]
[Route("api/prescricoes")]
[Authorize]
public class PrescricoesController : ControllerBase
{
    private readonly IPrescricaoService _prescricaoService;
    private readonly ILogAuditoriaService _logAuditoriaService;

    public PrescricoesController(IPrescricaoService prescricaoService, ILogAuditoriaService logAuditoriaService)
    {
        _prescricaoService = prescricaoService;
        _logAuditoriaService = logAuditoriaService;
    }

    /// <summary>
    /// Listar prescrições
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PrescricoesPaginadasDto>> Listar([FromQuery] FiltrosPrescricaoDto filtros)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        // Pacientes só veem suas próprias prescrições
        if (tipo == "Paciente")
        {
            filtros.PacienteId = usuarioId;
        }
        else if (tipo == "Profissional")
        {
            filtros.ProfissionalId = usuarioId;
        }

        var resultado = await _prescricaoService.ObterPrescricoesAsync(filtros);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter prescrição por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrescricaoDto>> ObterPorId(Guid id)
    {
        var prescricao = await _prescricaoService.ObterPrescricaoPorIdAsync(id);
        if (prescricao == null)
        {
            return NotFound(new { mensagem = "Prescrição não encontrada." });
        }

        return Ok(prescricao);
    }

    /// <summary>
    /// Obter prescrições por consulta
    /// </summary>
    [HttpGet("consulta/{consultaId:guid}")]
    public async Task<ActionResult<List<PrescricaoDto>>> ObterPorConsulta(Guid consultaId)
    {
        var prescricoes = await _prescricaoService.ObterPrescricoesPorConsultaAsync(consultaId);
        return Ok(prescricoes);
    }

    /// <summary>
    /// Criar prescrição
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<PrescricaoDto>> Criar([FromBody] CriarPrescricaoDto dto)
    {
        try
        {
            var prescricao = await _prescricaoService.CriarPrescricaoAsync(dto);

            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "criar",
                "Prescricao",
                prescricao.Id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return CreatedAtAction(nameof(ObterPorId), new { id = prescricao.Id }, prescricao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar prescrição
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<PrescricaoDto>> Atualizar(Guid id, [FromBody] AtualizarPrescricaoDto dto)
    {
        try
        {
            var prescricao = await _prescricaoService.AtualizarPrescricaoAsync(id, dto);
            if (prescricao == null)
            {
                return NotFound(new { mensagem = "Prescrição não encontrada." });
            }

            return Ok(prescricao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar prescrição
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        try
        {
            var sucesso = await _prescricaoService.DeletarPrescricaoAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Prescrição não encontrada." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Assinar prescrição digitalmente
    /// </summary>
    [HttpPost("{id:guid}/assinar")]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> Assinar(Guid id, [FromBody] AssinarPrescricaoDto dto)
    {
        try
        {
            var profissionalId = ObterUsuarioIdAtual();
            var sucesso = await _prescricaoService.AssinarPrescricaoAsync(id, profissionalId, dto.CertificadoId);
            
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Prescrição não encontrada." });
            }

            await _logAuditoriaService.RegistrarAsync(
                profissionalId,
                "assinar",
                "Prescricao",
                id,
                null,
                new { dto.CertificadoId },
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(new { mensagem = "Prescrição assinada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Gerar PDF da prescrição
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GerarPdf(Guid id)
    {
        try
        {
            var pdf = await _prescricaoService.GerarPdfPrescricaoAsync(id);
            return File(pdf, "application/pdf", $"prescricao_{id}.pdf");
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
/// DTO para assinar prescrição
/// </summary>
public class AssinarPrescricaoDto
{
    public string CertificadoId { get; set; } = string.Empty;
}
