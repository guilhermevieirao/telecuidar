using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Application.Common.Interfaces;
using app.Application.Cadsus.DTOs;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CadsusController : ControllerBase
{
    private readonly ICadsusService _cadsusService;
    private readonly ILogger<CadsusController> _logger;

    public CadsusController(ICadsusService cadsusService, ILogger<CadsusController> _logger)
    {
        _cadsusService = cadsusService;
        this._logger = _logger;
    }

    /// <summary>
    /// Verifica o status do token JWT do CADSUS
    /// </summary>
    [HttpGet("token/status")]
    public async Task<ActionResult<CadsusTokenStatus>> GetTokenStatus()
    {
        try
        {
            var status = await _cadsusService.GetTokenStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar status do token");
            return StatusCode(500, new { error = "Erro ao verificar status do token", message = ex.Message });
        }
    }

    /// <summary>
    /// Força a renovação manual do token JWT do CADSUS
    /// </summary>
    [HttpPost("token/renew")]
    public async Task<ActionResult> RenewToken()
    {
        try
        {
            _logger.LogInformation("🔄 Manual token renewal requested");
            await _cadsusService.RenewTokenAsync();
            var status = await _cadsusService.GetTokenStatusAsync();
            
            return Ok(new
            {
                success = true,
                message = "Token renovado com sucesso",
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(500, new
            {
                success = false,
                error = "Erro ao renovar token",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Consulta dados de um cidadão no CADSUS por CPF
    /// </summary>
    /// <param name="request">Objeto contendo o CPF a ser consultado</param>
    [HttpPost("consultar-cpf")]
    public async Task<ActionResult<CadsusCidadao>> ConsultarCpf([FromBody] ConsultarCpfRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Cpf))
            {
                return BadRequest(new { error = "CPF é obrigatório" });
            }

            _logger.LogInformation($"Received request for CPF: {request.Cpf}");

            var resultado = await _cadsusService.ConsultarCpfAsync(request.Cpf);

            _logger.LogInformation("📤 Sending response to frontend");
            _logger.LogInformation($"📋 Nome: {resultado.Nome}, CPF: {resultado.Cpf}, CNS: {resultado.Cns}");
            _logger.LogInformation($"📋 Data Nascimento: {resultado.DataNascimento}, Sexo: {resultado.Sexo}");
            
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "CPF inválido");
            return BadRequest(new { error = "CPF inválido", message = ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Certificado não encontrado");
            return StatusCode(500, new 
            { 
                error = "Erro de configuração", 
                message = "Certificado digital não encontrado. Verifique a configuração CADSUS__CertPath no .env" 
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Erro de configuração");
            return StatusCode(500, new
            {
                error = "Erro de configuração",
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in /api/cadsus/consultar-cpf");

            return StatusCode(500, new
            {
                error = "Erro ao consultar CADSUS",
                message = ex.Message
            });
        }
    }
}

public class ConsultarCpfRequest
{
    public string Cpf { get; set; } = "";
}
