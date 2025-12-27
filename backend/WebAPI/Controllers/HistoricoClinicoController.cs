using Application.DTOs.HistoricosClinico;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de histórico clínico
/// </summary>
[ApiController]
[Route("api/historico-clinico")]
[Authorize]
public class HistoricoClinicoController : ControllerBase
{
    private readonly IHistoricoClinicoService _historicoService;

    public HistoricoClinicoController(IHistoricoClinicoService historicoService)
    {
        _historicoService = historicoService;
    }

    /// <summary>
    /// Obter histórico clínico do paciente
    /// </summary>
    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<ActionResult<HistoricoClinicoDto>> ObterHistorico(Guid pacienteId)
    {
        // Verificar permissão
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var historico = await _historicoService.ObterHistoricoPorPacienteAsync(pacienteId);
        if (historico == null)
        {
            return NotFound(new { mensagem = "Histórico clínico não encontrado." });
        }

        return Ok(historico);
    }

    /// <summary>
    /// Obter meu histórico clínico
    /// </summary>
    [HttpGet("meu-historico")]
    public async Task<ActionResult<HistoricoClinicoDto>> ObterMeuHistorico()
    {
        var usuarioId = ObterUsuarioIdAtual();
        var historico = await _historicoService.ObterHistoricoPorPacienteAsync(usuarioId);
        
        if (historico == null)
        {
            return NotFound(new { mensagem = "Histórico clínico não encontrado." });
        }

        return Ok(historico);
    }

    /// <summary>
    /// Atualizar histórico clínico
    /// </summary>
    [HttpPut("paciente/{pacienteId:guid}")]
    public async Task<ActionResult<HistoricoClinicoDto>> AtualizarHistorico(Guid pacienteId, [FromBody] AtualizarHistoricoClinicoDto dto)
    {
        // Verificar permissão
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var historico = await _historicoService.CriarOuAtualizarHistoricoAsync(pacienteId, dto);
        return Ok(historico);
    }

    /// <summary>
    /// Obter histórico de consultas
    /// </summary>
    [HttpGet("paciente/{pacienteId:guid}/consultas")]
    public async Task<ActionResult<HistoricoConsultasDto>> ObterHistoricoConsultas(
        Guid pacienteId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        // Verificar permissão
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var historico = await _historicoService.ObterHistoricoConsultasAsync(pacienteId, pagina, tamanhoPagina);
        return Ok(historico);
    }

    /// <summary>
    /// Obter resumo clínico
    /// </summary>
    [HttpGet("paciente/{pacienteId:guid}/resumo")]
    public async Task<ActionResult<ResumoClinicoDto>> ObterResumo(Guid pacienteId)
    {
        // Verificar permissão
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var resumo = await _historicoService.ObterResumoClinicoAsync(pacienteId);
        return Ok(resumo);
    }

    /// <summary>
    /// Obter alergias do paciente
    /// </summary>
    [HttpGet("paciente/{pacienteId:guid}/alergias")]
    public async Task<ActionResult<List<AlergiaDto>>> ObterAlergias(Guid pacienteId)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var alergias = await _historicoService.ObterAlergiasAsync(pacienteId);
        return Ok(alergias);
    }

    /// <summary>
    /// Adicionar alergia
    /// </summary>
    [HttpPost("paciente/{pacienteId:guid}/alergias")]
    public async Task<IActionResult> AdicionarAlergia(Guid pacienteId, [FromBody] AlergiaDto alergia)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        await _historicoService.AdicionarAlergiaAsync(pacienteId, alergia);
        return Ok(new { mensagem = "Alergia adicionada com sucesso." });
    }

    /// <summary>
    /// Obter medicamentos em uso
    /// </summary>
    [HttpGet("paciente/{pacienteId:guid}/medicamentos")]
    public async Task<ActionResult<List<MedicamentoEmUsoDto>>> ObterMedicamentos(Guid pacienteId)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        var medicamentos = await _historicoService.ObterMedicamentosEmUsoAsync(pacienteId);
        return Ok(medicamentos);
    }

    /// <summary>
    /// Adicionar medicamento em uso
    /// </summary>
    [HttpPost("paciente/{pacienteId:guid}/medicamentos")]
    public async Task<IActionResult> AdicionarMedicamento(Guid pacienteId, [FromBody] MedicamentoEmUsoDto medicamento)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && usuarioId != pacienteId)
        {
            return Forbid();
        }

        await _historicoService.AdicionarMedicamentoEmUsoAsync(pacienteId, medicamento);
        return Ok(new { mensagem = "Medicamento adicionado com sucesso." });
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
}
