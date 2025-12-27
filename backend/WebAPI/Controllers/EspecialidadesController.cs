using Application.DTOs.Especialidades;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de especialidades
/// </summary>
[ApiController]
[Route("api/especialidades")]
public class EspecialidadesController : ControllerBase
{
    private readonly IEspecialidadeService _especialidadeService;

    public EspecialidadesController(IEspecialidadeService especialidadeService)
    {
        _especialidadeService = especialidadeService;
    }

    /// <summary>
    /// Listar especialidades
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<EspecialidadesPaginadasDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        [FromQuery] string? busca = null,
        [FromQuery] string? status = null)
    {
        var resultado = await _especialidadeService.ObterEspecialidadesAsync(pagina, tamanhoPagina, busca, status);
        return Ok(resultado);
    }

    /// <summary>
    /// Listar especialidades ativas (para seleção)
    /// </summary>
    [HttpGet("ativas")]
    public async Task<ActionResult<List<EspecialidadeDto>>> ListarAtivas()
    {
        var especialidades = await _especialidadeService.ObterTodasEspecialidadesAtivasAsync();
        return Ok(especialidades);
    }

    /// <summary>
    /// Obter especialidade por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EspecialidadeDto>> ObterPorId(Guid id)
    {
        var especialidade = await _especialidadeService.ObterEspecialidadePorIdAsync(id);
        if (especialidade == null)
        {
            return NotFound(new { mensagem = "Especialidade não encontrada." });
        }

        return Ok(especialidade);
    }

    /// <summary>
    /// Criar especialidade
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<EspecialidadeDto>> Criar([FromBody] CriarEspecialidadeDto dto)
    {
        try
        {
            var especialidade = await _especialidadeService.CriarEspecialidadeAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = especialidade.Id }, especialidade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar especialidade
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<EspecialidadeDto>> Atualizar(Guid id, [FromBody] AtualizarEspecialidadeDto dto)
    {
        try
        {
            var especialidade = await _especialidadeService.AtualizarEspecialidadeAsync(id, dto);
            if (especialidade == null)
            {
                return NotFound(new { mensagem = "Especialidade não encontrada." });
            }

            return Ok(especialidade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar especialidade
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _especialidadeService.DeletarEspecialidadeAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Especialidade não encontrada." });
        }

        return NoContent();
    }

    /// <summary>
    /// Ativar especialidade
    /// </summary>
    [HttpPost("{id:guid}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var sucesso = await _especialidadeService.AtivarEspecialidadeAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Especialidade não encontrada." });
        }

        return Ok(new { mensagem = "Especialidade ativada com sucesso." });
    }

    /// <summary>
    /// Desativar especialidade
    /// </summary>
    [HttpPost("{id:guid}/desativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var sucesso = await _especialidadeService.DesativarEspecialidadeAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Especialidade não encontrada." });
        }

        return Ok(new { mensagem = "Especialidade desativada com sucesso." });
    }
}
