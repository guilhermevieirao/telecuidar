using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de saúde da aplicação (health check)
/// </summary>
[ApiController]
[Route("api/saude")]
public class SaudeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SaudeController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Verificar saúde da aplicação
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Verificar()
    {
        var status = new SaudeStatusDto
        {
            Status = "Saudável",
            Versao = "1.0.0",
            DataHora = DateTime.UtcNow
        };

        // Verificar banco de dados
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            status.BancoDados = "Conectado";
        }
        catch
        {
            status.BancoDados = "Desconectado";
            status.Status = "Degradado";
        }

        return Ok(status);
    }

    /// <summary>
    /// Verificação de prontidão (readiness)
    /// </summary>
    [HttpGet("pronto")]
    [AllowAnonymous]
    public async Task<IActionResult> Pronto()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            return Ok(new { pronto = true });
        }
        catch
        {
            return StatusCode(503, new { pronto = false, mensagem = "Banco de dados indisponível." });
        }
    }

    /// <summary>
    /// Verificação de vida (liveness)
    /// </summary>
    [HttpGet("vivo")]
    [AllowAnonymous]
    public IActionResult Vivo()
    {
        return Ok(new { vivo = true });
    }

    /// <summary>
    /// Obter informações do sistema
    /// </summary>
    [HttpGet("info")]
    [Authorize(Roles = "Administrador")]
    public IActionResult Info()
    {
        var info = new InfoSistemaDto
        {
            Versao = "1.0.0",
            Ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            DataHora = DateTime.UtcNow,
            TempoAtivo = TimeSpan.FromMilliseconds(Environment.TickCount64),
            DotNetVersion = Environment.Version.ToString(),
            Sistema = Environment.OSVersion.ToString()
        };

        return Ok(info);
    }
}

/// <summary>
/// Status de saúde da aplicação
/// </summary>
public class SaudeStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string Versao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string BancoDados { get; set; } = string.Empty;
}

/// <summary>
/// Informações do sistema
/// </summary>
public class InfoSistemaDto
{
    public string Versao { get; set; } = string.Empty;
    public string Ambiente { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public TimeSpan TempoAtivo { get; set; }
    public string DotNetVersion { get; set; } = string.Empty;
    public string Sistema { get; set; } = string.Empty;
}
