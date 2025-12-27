using Application.DTOs.Medicamentos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de medicamentos ANVISA
/// </summary>
[ApiController]
[Route("api/medicamentos")]
[Authorize]
public class MedicamentosAnvisaController : ControllerBase
{
    private readonly IMedicamentoAnvisaService _medicamentoService;

    public MedicamentosAnvisaController(IMedicamentoAnvisaService medicamentoService)
    {
        _medicamentoService = medicamentoService;
    }

    /// <summary>
    /// Buscar medicamentos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<MedicamentosPaginadosDto>> Buscar(
        [FromQuery] string termo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20)
    {
        if (string.IsNullOrEmpty(termo) || termo.Length < 3)
        {
            return BadRequest(new { mensagem = "O termo de busca deve ter pelo menos 3 caracteres." });
        }

        var resultado = await _medicamentoService.BuscarMedicamentosAsync(termo, pagina, tamanhoPagina);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter medicamento por registro
    /// </summary>
    [HttpGet("{registro}")]
    public async Task<ActionResult<MedicamentoAnvisaDto>> ObterPorRegistro(string registro)
    {
        var medicamento = await _medicamentoService.ObterMedicamentoPorRegistroAsync(registro);
        if (medicamento == null)
        {
            return NotFound(new { mensagem = "Medicamento não encontrado." });
        }

        return Ok(medicamento);
    }

    /// <summary>
    /// Buscar princípios ativos
    /// </summary>
    [HttpGet("principios-ativos")]
    public async Task<ActionResult<List<string>>> BuscarPrincipiosAtivos([FromQuery] string termo)
    {
        if (string.IsNullOrEmpty(termo) || termo.Length < 2)
        {
            return BadRequest(new { mensagem = "O termo de busca deve ter pelo menos 2 caracteres." });
        }

        var principios = await _medicamentoService.ObterPrincipiosAtivosAsync(termo);
        return Ok(principios);
    }

    /// <summary>
    /// Buscar por princípio ativo
    /// </summary>
    [HttpGet("por-principio-ativo")]
    public async Task<ActionResult<List<MedicamentoAnvisaDto>>> BuscarPorPrincipioAtivo([FromQuery] string principioAtivo)
    {
        if (string.IsNullOrEmpty(principioAtivo))
        {
            return BadRequest(new { mensagem = "O princípio ativo é obrigatório." });
        }

        var medicamentos = await _medicamentoService.BuscarPorPrincipioAtivoAsync(principioAtivo);
        return Ok(medicamentos);
    }
}
