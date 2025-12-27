using Application.DTOs.Jitsi;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de integração Jitsi Meet
/// </summary>
[ApiController]
[Route("api/jitsi")]
[Authorize]
public class JitsiController : ControllerBase
{
    private readonly IJitsiService _jitsiService;
    private readonly IConsultaService _consultaService;

    public JitsiController(IJitsiService jitsiService, IConsultaService consultaService)
    {
        _jitsiService = jitsiService;
        _consultaService = consultaService;
    }

    /// <summary>
    /// Criar sala para consulta
    /// </summary>
    [HttpPost("sala")]
    public async Task<ActionResult<SalaJitsiDto>> CriarSala([FromBody] CriarSalaJitsiDto dto)
    {
        var sala = await _jitsiService.CriarSalaAsync(dto);
        return Ok(sala);
    }

    /// <summary>
    /// Gerar token JWT para entrar na sala
    /// </summary>
    [HttpPost("token")]
    public async Task<ActionResult<TokenJitsiDto>> GerarToken([FromBody] GerarTokenJitsiRequestDto dto)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var usuario = await ObterNomeUsuarioAtual();

        var tokenDto = new GerarTokenJitsiDto
        {
            NomeSala = dto.NomeSala,
            UsuarioId = usuarioId,
            NomeUsuario = usuario.Nome,
            EmailUsuario = usuario.Email,
            Moderador = dto.Moderador
        };

        var token = await _jitsiService.GerarTokenAsync(tokenDto);
        return Ok(token);
    }

    /// <summary>
    /// Obter token para consulta (profissional como moderador)
    /// </summary>
    [HttpGet("consulta/{consultaId:guid}/token")]
    public async Task<ActionResult<TokenJitsiDto>> ObterTokenParaConsulta(Guid consultaId)
    {
        var consulta = await _consultaService.ObterConsultaPorIdAsync(consultaId);
        if (consulta == null)
        {
            return NotFound(new { mensagem = "Consulta não encontrada." });
        }

        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        // Verificar se o usuário faz parte da consulta
        if (tipo == "Paciente" && consulta.PacienteId != usuarioId)
        {
            return Forbid();
        }
        if (tipo == "Profissional" && consulta.ProfissionalId != usuarioId)
        {
            return Forbid();
        }

        var usuario = await ObterNomeUsuarioAtual();
        var ehModerador = tipo == "Profissional" || tipo == "Administrador";

        // Usar ID da consulta como nome da sala
        var nomeSala = $"consulta_{consultaId}";

        TokenJitsiDto token;
        if (ehModerador)
        {
            token = await _jitsiService.GerarTokenModeradoAsync(nomeSala, usuarioId, usuario.Nome, usuario.Email);
        }
        else
        {
            token = await _jitsiService.GerarTokenParticipanteAsync(nomeSala, usuarioId, usuario.Nome, usuario.Email);
        }

        return Ok(token);
    }

    /// <summary>
    /// Obter configurações do Jitsi
    /// </summary>
    [HttpGet("configuracao")]
    public ActionResult<ConfiguracaoJitsiDto> ObterConfiguracao()
    {
        var config = _jitsiService.ObterConfiguracao();
        return Ok(config);
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

    private Task<(string Nome, string Email)> ObterNomeUsuarioAtual()
    {
        var nome = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuário";
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
        return Task.FromResult((nome, email));
    }
}

/// <summary>
/// DTO para requisição de token
/// </summary>
public class GerarTokenJitsiRequestDto
{
    public string NomeSala { get; set; } = string.Empty;
    public bool Moderador { get; set; }
}
