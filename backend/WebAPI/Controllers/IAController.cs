using Application.DTOs.AI;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de integração com IA
/// </summary>
[ApiController]
[Route("api/ia")]
[Authorize(Roles = "Profissional,Administrador")]
public class IAController : ControllerBase
{
    private readonly IIAService _iaService;

    public IAController(IIAService iaService)
    {
        _iaService = iaService;
    }

    /// <summary>
    /// Gerar resumo da consulta
    /// </summary>
    [HttpPost("resumo-consulta")]
    public async Task<ActionResult<RespostaIADto>> GerarResumo([FromBody] DadosConsultaIADto dados)
    {
        var resposta = await _iaService.GerarResumoConsultaAsync(dados);
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Sugerir diagnósticos
    /// </summary>
    [HttpPost("sugerir-diagnostico")]
    public async Task<ActionResult<RespostaIADto>> SugerirDiagnostico([FromBody] DadosConsultaIADto dados)
    {
        var resposta = await _iaService.SugerirDiagnosticoAsync(dados);
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Sugerir conduta
    /// </summary>
    [HttpPost("sugerir-conduta")]
    public async Task<ActionResult<RespostaIADto>> SugerirConduta([FromBody] DadosConsultaIADto dados)
    {
        var resposta = await _iaService.SugerirCondutaAsync(dados);
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Gerar texto para atestado
    /// </summary>
    [HttpPost("texto-atestado")]
    public async Task<ActionResult<RespostaIADto>> GerarTextoAtestado([FromBody] GerarTextoAtestadoDto dto)
    {
        var resposta = await _iaService.GerarTextoAtestadoAsync(dto.Tipo, dto.Dias, dto.Condicao);
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Analisar sintomas (uso informativo)
    /// </summary>
    [HttpPost("analisar-sintomas")]
    [AllowAnonymous]
    public async Task<ActionResult<RespostaIADto>> AnalisarSintomas([FromBody] AnalisarSintomasDto dto)
    {
        var resposta = await _iaService.AnalisarSintomasAsync(dto.Sintomas, dto.Biometricos);
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Chat com assistente
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<RespostaIADto>> Chat([FromBody] ChatIADto dto)
    {
        var resposta = await _iaService.ProcessarMensagemAsync(dto.Mensagem, dto.Contexto ?? "");
        if (!resposta.Sucesso)
        {
            return StatusCode(500, new { mensagem = resposta.Erro });
        }

        return Ok(resposta);
    }
}

/// <summary>
/// DTO para gerar texto de atestado
/// </summary>
public class GerarTextoAtestadoDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Dias { get; set; }
    public string? Condicao { get; set; }
}

/// <summary>
/// DTO para analisar sintomas
/// </summary>
public class AnalisarSintomasDto
{
    public List<string> Sintomas { get; set; } = new();
    public DadosBiometricosIADto? Biometricos { get; set; }
}

/// <summary>
/// DTO para chat com IA
/// </summary>
public class ChatIADto
{
    public string Mensagem { get; set; } = string.Empty;
    public string? Contexto { get; set; }
}
