using Application.DTOs.Auth;
using Application.DTOs.Usuarios;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de autenticação
/// </summary>
[ApiController]
[Route("api/autenticacao")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly ILogAuditoriaService _logAuditoriaService;
    private readonly IUsuarioService _usuarioService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService, 
        ILogAuditoriaService logAuditoriaService, 
        IUsuarioService usuarioService)
    {
        _autenticacaoService = autenticacaoService;
        _logAuditoriaService = logAuditoriaService;
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Registrar novo usuário
    /// </summary>
    [HttpPost("registrar")]
    public async Task<ActionResult<RegistroResponseDto>> Registrar([FromBody] RegistroRequestDto request)
    {
        try
        {
            if (request.Senha != request.ConfirmarSenha)
            {
                return BadRequest(new { mensagem = "As senhas não coincidem." });
            }

            if (!request.AceitarTermos)
            {
                return BadRequest(new { mensagem = "Você deve aceitar os termos e condições." });
            }

            var usuario = await _autenticacaoService.RegistrarAsync(
                request.Nome, 
                request.Sobrenome, 
                request.Email, 
                request.Cpf, 
                request.Telefone, 
                request.Senha, 
                request.TokenConvite);

            // Buscar usuário completo
            var usuarioDto = await _usuarioService.ObterUsuarioPorIdAsync(usuario.Id);

            var response = new RegistroResponseDto
            {
                Usuario = usuarioDto!,
                Mensagem = "Usuário registrado com sucesso. Por favor, verifique seu e-mail."
            };

            // Log de auditoria
            await _logAuditoriaService.RegistrarAsync(
                usuario.Id,
                "registrar",
                "Usuario",
                usuario.Id,
                null,
                new { usuario.Email, usuario.Nome, usuario.Sobrenome, Tipo = usuario.Tipo.ToString() },
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Ocorreu um erro durante o registro.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var loginResponse = await _autenticacaoService.LoginAsync(
                request.Email,
                request.Senha,
                request.LembrarMe
            );

            if (loginResponse == null)
            {
                return Unauthorized(new { mensagem = "Credenciais inválidas." });
            }

            // Log de auditoria
            await _logAuditoriaService.RegistrarAsync(
                loginResponse.Usuario.Id,
                "login",
                "Usuario",
                loginResponse.Usuario.Id,
                null,
                new { loginResponse.Usuario.Email },
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(loginResponse);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Ocorreu um erro durante o login.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Renovar token
    /// </summary>
    [HttpPost("renovar-token")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RenovarToken([FromBody] RenovarTokenRequestDto request)
    {
        try
        {
            var resultado = await _autenticacaoService.RenovarTokenAsync(request.RefreshToken);
            
            if (resultado == null)
            {
                return Unauthorized(new { mensagem = "Token inválido ou expirado." });
            }

            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "logout",
                "Usuario",
                ObterUsuarioIdAtual(),
                null,
                null,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(new { mensagem = "Logout realizado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao fazer logout.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Solicitar redefinição de senha
    /// </summary>
    [HttpPost("esqueci-senha")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequestDto request)
    {
        try
        {
            await _autenticacaoService.EsqueciSenhaAsync(request.Email);
            return Ok(new { mensagem = "Se o e-mail existir, você receberá instruções para redefinir sua senha." });
        }
        catch
        {
            // Retornar sucesso mesmo em caso de erro para não revelar se o email existe
            return Ok(new { mensagem = "Se o e-mail existir, você receberá instruções para redefinir sua senha." });
        }
    }

    /// <summary>
    /// Redefinir senha
    /// </summary>
    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequestDto request)
    {
        try
        {
            if (request.NovaSenha != request.ConfirmarSenha)
            {
                return BadRequest(new { mensagem = "As senhas não coincidem." });
            }

            await _autenticacaoService.RedefinirSenhaAsync(request.Token, request.NovaSenha);
            return Ok(new { mensagem = "Senha redefinida com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Verificar e-mail
    /// </summary>
    [HttpGet("verificar-email")]
    public async Task<IActionResult> VerificarEmail([FromQuery] string token)
    {
        try
        {
            await _autenticacaoService.VerificarEmailAsync(token);
            return Ok(new { mensagem = "E-mail verificado com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Reenviar e-mail de verificação
    /// </summary>
    [HttpPost("reenviar-verificacao")]
    public async Task<IActionResult> ReenviarVerificacao([FromBody] ReenviarVerificacaoRequestDto request)
    {
        try
        {
            await _autenticacaoService.ReenviarEmailVerificacaoAsync(request.Email);
            return Ok(new { mensagem = "E-mail de verificação reenviado." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Alterar senha (autenticado)
    /// </summary>
    [HttpPost("alterar-senha")]
    [Authorize]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequestDto request)
    {
        try
        {
            if (request.NovaSenha != request.ConfirmarSenha)
            {
                return BadRequest(new { mensagem = "As senhas não coincidem." });
            }

            await _autenticacaoService.AlterarSenhaAsync(ObterUsuarioIdAtual(), request.SenhaAtual, request.NovaSenha);
            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obter usuário atual
    /// </summary>
    [HttpGet("eu")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> ObterUsuarioAtual()
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorIdAsync(ObterUsuarioIdAtual());
            if (usuario == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter usuário.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Verificar se email está disponível
    /// </summary>
    [HttpGet("verificar-email-disponivel/{email}")]
    public async Task<ActionResult<object>> VerificarEmailDisponivel(string email)
    {
        try
        {
            var emailDecodificado = Uri.UnescapeDataString(email);
            var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(emailDecodificado);
            return Ok(new { available = usuario == null });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao verificar email.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Verificar se CPF está disponível
    /// </summary>
    [HttpGet("verificar-cpf-disponivel/{cpf}")]
    public async Task<ActionResult<object>> VerificarCpfDisponivel(string cpf)
    {
        try
        {
            var cpfDecodificado = Uri.UnescapeDataString(cpf);
            var usuario = await _usuarioService.ObterUsuarioPorCpfAsync(cpfDecodificado);
            return Ok(new { available = usuario == null });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao verificar CPF.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Verificar se telefone está disponível
    /// </summary>
    [HttpGet("verificar-telefone-disponivel/{telefone}")]
    public async Task<ActionResult<object>> VerificarTelefoneDisponivel(string telefone)
    {
        try
        {
            var telefoneDecodificado = Uri.UnescapeDataString(telefone);
            var usuario = await _usuarioService.ObterUsuarioPorTelefoneAsync(telefoneDecodificado);
            return Ok(new { available = usuario == null });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao verificar telefone.", erro = ex.Message });
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
