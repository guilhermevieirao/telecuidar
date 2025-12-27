using Application.DTOs.Cns;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de validação de CNS
/// </summary>
[ApiController]
[Route("api/cns")]
[Authorize]
public class CnsController : ControllerBase
{
    private readonly ICnsService _cnsService;

    public CnsController(ICnsService cnsService)
    {
        _cnsService = cnsService;
    }

    /// <summary>
    /// Validar CNS
    /// </summary>
    [HttpGet("validar/{cns}")]
    public ActionResult<ValidarCnsResponseDto> Validar(string cns)
    {
        var resultado = _cnsService.ValidarCns(cns);
        return Ok(resultado);
    }

    /// <summary>
    /// Buscar informações do CNS (DataSUS)
    /// </summary>
    [HttpGet("buscar/{cns}")]
    public async Task<ActionResult<InfoCnsResponseDto>> Buscar(string cns)
    {
        var validacao = _cnsService.ValidarCns(cns);
        if (!validacao.Valido)
        {
            return BadRequest(new { mensagem = validacao.Mensagem });
        }

        var info = await _cnsService.BuscarInfoCnsAsync(cns);
        if (info == null)
        {
            return NotFound(new { mensagem = "Informações não encontradas." });
        }

        return Ok(info);
    }
}
