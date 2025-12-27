using Application.DTOs.Usuarios;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de usuários
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogAuditoriaService _logAuditoriaService;

    public UsuariosController(IUsuarioService usuarioService, ILogAuditoriaService logAuditoriaService)
    {
        _usuarioService = usuarioService;
        _logAuditoriaService = logAuditoriaService;
    }

    /// <summary>
    /// Listar usuários com paginação
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuariosPaginadosDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? tipo = null,
        [FromQuery] string? status = null,
        [FromQuery] string? busca = null)
    {
        var resultado = await _usuarioService.ObterUsuariosAsync(pagina, tamanhoPagina, busca, tipo, status);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter usuário por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> ObterPorId(Guid id)
    {
        // Verificar se é o próprio usuário ou admin
        var usuarioAtualId = ObterUsuarioIdAtual();
        var ehAdmin = User.IsInRole("Administrador");

        if (usuarioAtualId != id && !ehAdmin)
        {
            return Forbid();
        }

        var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
        if (usuario == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        return Ok(usuario);
    }

    /// <summary>
    /// Atualizar usuário
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> Atualizar(Guid id, [FromBody] AtualizarUsuarioDto dto)
    {
        var usuarioAtualId = ObterUsuarioIdAtual();
        var ehAdmin = User.IsInRole("Administrador");

        if (usuarioAtualId != id && !ehAdmin)
        {
            return Forbid();
        }

        try
        {
            var usuario = await _usuarioService.AtualizarUsuarioAsync(id, dto);
            if (usuario == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            await _logAuditoriaService.RegistrarAsync(
                usuarioAtualId,
                "atualizar",
                "Usuario",
                id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar avatar
    /// </summary>
    [HttpPut("{id:guid}/avatar")]
    public async Task<ActionResult> AtualizarAvatar(Guid id, IFormFile arquivo)
    {
        var usuarioAtualId = ObterUsuarioIdAtual();
        var ehAdmin = User.IsInRole("Administrador");

        if (usuarioAtualId != id && !ehAdmin)
        {
            return Forbid();
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await arquivo.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            var avatarUrl = await _usuarioService.AtualizarAvatarAsync(id, base64);
            
            return Ok(new { avatar = avatarUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Deletar usuário
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _usuarioService.DeletarUsuarioAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        await _logAuditoriaService.RegistrarAsync(
            ObterUsuarioIdAtual(),
            "deletar",
            "Usuario",
            id,
            null,
            null,
            ObterEnderecoIp(),
            ObterUserAgent()
        );

        return NoContent();
    }

    /// <summary>
    /// Ativar usuário
    /// </summary>
    [HttpPost("{id:guid}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var usuario = await _usuarioService.AtualizarUsuarioAsync(id, new AtualizarUsuarioDto { Status = "Ativo" });
        if (usuario == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        return Ok(new { mensagem = "Usuário ativado com sucesso." });
    }

    /// <summary>
    /// Desativar usuário
    /// </summary>
    [HttpPost("{id:guid}/desativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var usuario = await _usuarioService.AtualizarUsuarioAsync(id, new AtualizarUsuarioDto { Status = "Inativo" });
        if (usuario == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        return Ok(new { mensagem = "Usuário desativado com sucesso." });
    }

    /// <summary>
    /// Alterar tipo de usuário
    /// </summary>
    [HttpPost("{id:guid}/alterar-tipo")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioDto>> AlterarTipo(Guid id, [FromBody] AlterarTipoUsuarioDto dto)
    {
        try
        {
            var usuario = await _usuarioService.AtualizarUsuarioAsync(id, new AtualizarUsuarioDto { Tipo = dto.NovoTipo });
            if (usuario == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "alterar_tipo",
                "Usuario",
                id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Listar profissionais
    /// </summary>
    [HttpGet("profissionais")]
    public async Task<ActionResult<UsuariosPaginadosDto>> ListarProfissionais(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] Guid? especialidadeId = null)
    {
        var resultado = await _usuarioService.ObterUsuariosAsync(pagina, tamanhoPagina, null, "Profissional", "Ativo", especialidadeId);
        return Ok(resultado);
    }

    /// <summary>
    /// Listar pacientes
    /// </summary>
    [HttpGet("pacientes")]
    [Authorize(Roles = "Administrador,Profissional")]
    public async Task<ActionResult<UsuariosPaginadosDto>> ListarPacientes(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? busca = null)
    {
        var resultado = await _usuarioService.ObterUsuariosAsync(pagina, tamanhoPagina, busca, "Paciente", null);
        return Ok(resultado);
    }

    /// <summary>
    /// Atualizar perfil de paciente
    /// </summary>
    [HttpPut("{id:guid}/perfil-paciente")]
    public async Task<ActionResult<PerfilPacienteDto>> AtualizarPerfilPaciente(Guid id, [FromBody] CriarPerfilPacienteDto dto)
    {
        var usuarioAtualId = ObterUsuarioIdAtual();
        var ehAdmin = User.IsInRole("Administrador");

        if (usuarioAtualId != id && !ehAdmin)
        {
            return Forbid();
        }

        try
        {
            var perfil = await _usuarioService.CriarOuAtualizarPerfilPacienteAsync(id, dto);
            return Ok(perfil);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar perfil profissional
    /// </summary>
    [HttpPut("{id:guid}/perfil-profissional")]
    public async Task<ActionResult<PerfilProfissionalDto>> AtualizarPerfilProfissional(Guid id, [FromBody] CriarPerfilProfissionalDto dto)
    {
        var usuarioAtualId = ObterUsuarioIdAtual();
        var ehAdmin = User.IsInRole("Administrador");

        if (usuarioAtualId != id && !ehAdmin)
        {
            return Forbid();
        }

        try
        {
            var perfil = await _usuarioService.CriarOuAtualizarPerfilProfissionalAsync(id, dto);
            return Ok(perfil);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
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
/// DTO para alterar tipo de usuário
/// </summary>
public class AlterarTipoUsuarioDto
{
    public string NovoTipo { get; set; } = string.Empty;
}
