using Application.DTOs.Auth;
using Application.DTOs.Usuarios;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de autenticação
/// </summary>
public class AutenticacaoService : IAutenticacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IHashSenhaService _hashSenhaService;
    private readonly IEmailService _emailService;
    private readonly IConviteService _conviteService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AutenticacaoService> _logger;
    private readonly string _frontendUrl;

    public AutenticacaoService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IHashSenhaService hashSenhaService,
        IEmailService emailService,
        IConviteService conviteService,
        IConfiguration configuration,
        ILogger<AutenticacaoService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _hashSenhaService = hashSenhaService;
        _emailService = emailService;
        _conviteService = conviteService;
        _configuration = configuration;
        _logger = logger;
        _frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string senha, bool lembrarMe)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
                .ThenInclude(p => p!.Especialidade)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null || !_hashSenhaService.VerificarHash(senha, usuario.SenhaHash))
        {
            return null;
        }

        if (usuario.Status == StatusUsuario.Inativo)
        {
            throw new InvalidOperationException("Conta de usuário inativa.");
        }

        if (!usuario.EmailVerificado)
        {
            throw new InvalidOperationException("E-mail não verificado. Por favor, acesse sua caixa de entrada e clique no link de confirmação.");
        }

        var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
        var accessToken = _jwtService.GerarAccessToken(usuario.Id, usuario.Email, nomeCompleto, usuario.Tipo.ToString());
        var refreshToken = _jwtService.GerarRefreshToken();

        var diasRefresh = lembrarMe ? 30 : 7;
        await _jwtService.SalvarRefreshTokenAsync(usuario.Id, refreshToken, TimeSpan.FromDays(diasRefresh));

        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpira = DateTime.UtcNow.AddDays(diasRefresh);
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            Usuario = MapearUsuarioParaDto(usuario),
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<Usuario> RegistrarAsync(string nome, string sobrenome, string email, string cpf, string? telefone, string senha, string? tokenConvite = null)
    {
        // Validar email único
        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            throw new InvalidOperationException("E-mail já está em uso.");
        }

        // Validar CPF único
        if (await _context.Usuarios.AnyAsync(u => u.Cpf == cpf))
        {
            throw new InvalidOperationException("CPF já está em uso.");
        }

        // Validar telefone único
        if (!string.IsNullOrEmpty(telefone) && await _context.Usuarios.AnyAsync(u => u.Telefone == telefone))
        {
            throw new InvalidOperationException("Telefone já está em uso.");
        }

        // Determinar tipo de usuário
        var tipoUsuario = TipoUsuario.Paciente;
        Guid? especialidadeId = null;

        // Se é o primeiro usuário, torna-se admin
        var primeiroUsuario = !await _context.Usuarios.AnyAsync();
        if (primeiroUsuario)
        {
            tipoUsuario = TipoUsuario.Administrador;
        }
        else if (!string.IsNullOrEmpty(tokenConvite))
        {
            var convite = await _conviteService.ObterConvitePorTokenAsync(tokenConvite);
            if (convite != null && convite.Status == "Pendente")
            {
                tipoUsuario = Enum.Parse<TipoUsuario>(convite.TipoUsuario);
                especialidadeId = convite.EspecialidadeId;
            }
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Sobrenome = sobrenome,
            Email = email,
            Cpf = cpf,
            Telefone = telefone,
            SenhaHash = _hashSenhaService.GerarHash(senha),
            Tipo = tipoUsuario,
            Status = StatusUsuario.Ativo,
            EmailVerificado = false,
            TokenVerificacaoEmail = Guid.NewGuid().ToString(),
            TokenVerificacaoEmailExpira = DateTime.UtcNow.AddHours(24),
            CriadoEm = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);

        // Criar perfil baseado no tipo
        if (tipoUsuario == TipoUsuario.Paciente)
        {
            var perfilPaciente = new PerfilPaciente
            {
                UsuarioId = usuario.Id,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisPaciente.Add(perfilPaciente);
        }
        else if (tipoUsuario == TipoUsuario.Profissional)
        {
            var perfilProfissional = new PerfilProfissional
            {
                UsuarioId = usuario.Id,
                EspecialidadeId = especialidadeId,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisProfissional.Add(perfilProfissional);
        }

        await _context.SaveChangesAsync();

        // Aceitar convite se houver
        if (!string.IsNullOrEmpty(tokenConvite))
        {
            await _conviteService.AceitarConviteAsync(tokenConvite, usuario.Id);
        }

        // Enviar e-mail de verificação
        _ = Task.Run(async () =>
        {
            try
            {
                var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
                await _emailService.EnviarEmailVerificacaoAsync(usuario.Id, usuario.Email, nomeCompleto, usuario.TokenVerificacaoEmail!);
                _logger.LogInformation("E-mail de verificação enviado para {Email}", usuario.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de verificação para {Email}", usuario.Email);
            }
        });

        return usuario;
    }

    public async Task<RefreshTokenResponseDto?> RenovarTokenAsync(string refreshToken)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (usuario == null || usuario.RefreshTokenExpira == null || usuario.RefreshTokenExpira < DateTime.UtcNow)
        {
            return null;
        }

        var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
        var novoAccessToken = _jwtService.GerarAccessToken(usuario.Id, usuario.Email, nomeCompleto, usuario.Tipo.ToString());
        var novoRefreshToken = _jwtService.GerarRefreshToken();

        usuario.RefreshToken = novoRefreshToken;
        usuario.RefreshTokenExpira = DateTime.UtcNow.AddDays(7);
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new RefreshTokenResponseDto
        {
            AccessToken = novoAccessToken,
            RefreshToken = novoRefreshToken
        };
    }

    public async Task<bool> EsqueciSenhaAsync(string email)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            return true; // Por segurança, retorna true mesmo se não existe
        }

        usuario.TokenRedefinicaoSenha = Guid.NewGuid().ToString();
        usuario.TokenRedefinicaoSenhaExpira = DateTime.UtcNow.AddHours(1);
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
                await _emailService.EnviarEmailRedefinicaoSenhaAsync(usuario.Email, nomeCompleto, usuario.TokenRedefinicaoSenha);
                _logger.LogInformation("E-mail de redefinição enviado para {Email}", usuario.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de redefinição para {Email}", usuario.Email);
            }
        });

        return true;
    }

    public async Task<bool> RedefinirSenhaAsync(string token, string novaSenha)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRedefinicaoSenha == token);

        if (usuario == null || usuario.TokenRedefinicaoSenhaExpira == null || usuario.TokenRedefinicaoSenhaExpira < DateTime.UtcNow)
        {
            return false;
        }

        usuario.SenhaHash = _hashSenhaService.GerarHash(novaSenha);
        usuario.TokenRedefinicaoSenha = null;
        usuario.TokenRedefinicaoSenhaExpira = null;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerificarEmailAsync(string token)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenVerificacaoEmail == token);

        if (usuario == null || usuario.TokenVerificacaoEmailExpira == null || usuario.TokenVerificacaoEmailExpira < DateTime.UtcNow)
        {
            return false;
        }

        usuario.EmailVerificado = true;
        usuario.TokenVerificacaoEmail = null;
        usuario.TokenVerificacaoEmailExpira = null;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Usuario?> VerificarEmailComUsuarioAsync(string token)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
            .FirstOrDefaultAsync(u => u.TokenVerificacaoEmail == token);

        if (usuario == null || usuario.TokenVerificacaoEmailExpira == null || usuario.TokenVerificacaoEmailExpira < DateTime.UtcNow)
        {
            return null;
        }

        if (usuario.EmailVerificado)
        {
            return usuario;
        }

        usuario.EmailVerificado = true;
        usuario.TokenVerificacaoEmail = null;
        usuario.TokenVerificacaoEmailExpira = null;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ReenviarEmailVerificacaoAsync(string email)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null || usuario.EmailVerificado)
        {
            return false;
        }

        usuario.TokenVerificacaoEmail = Guid.NewGuid().ToString();
        usuario.TokenVerificacaoEmailExpira = DateTime.UtcNow.AddHours(24);
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
                await _emailService.EnviarEmailVerificacaoAsync(usuario.Id, usuario.Email, nomeCompleto, usuario.TokenVerificacaoEmail!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reenviar e-mail de verificação para {Email}", usuario.Email);
            }
        });

        return true;
    }

    public async Task<bool> AlterarSenhaAsync(Guid usuarioId, string senhaAtual, string novaSenha)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        if (usuario == null || !_hashSenhaService.VerificarHash(senhaAtual, usuario.SenhaHash))
        {
            return false;
        }

        usuario.SenhaHash = _hashSenhaService.GerarHash(novaSenha);
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailDisponivelAsync(string email)
    {
        return !await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> CpfDisponivelAsync(string cpf)
    {
        return !await _context.Usuarios.AnyAsync(u => u.Cpf == cpf);
    }

    public async Task<bool> TelefoneDisponivelAsync(string telefone)
    {
        return !await _context.Usuarios.AnyAsync(u => u.Telefone == telefone);
    }

    public async Task<bool> SolicitarTrocaEmailAsync(Guid usuarioId, string novoEmail)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == novoEmail))
        {
            throw new InvalidOperationException("E-mail já está em uso.");
        }

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            return false;
        }

        usuario.NovoEmailPendente = novoEmail;
        usuario.TokenTrocaEmail = Guid.NewGuid().ToString();
        usuario.TokenTrocaEmailExpira = DateTime.UtcNow.AddHours(24);
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                var nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
                await _emailService.EnviarEmailTrocaEmailAsync(novoEmail, nomeCompleto, usuario.TokenTrocaEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de troca para {Email}", novoEmail);
            }
        });

        return true;
    }

    public async Task<Usuario?> VerificarTrocaEmailAsync(string token)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenTrocaEmail == token);

        if (usuario == null || usuario.TokenTrocaEmailExpira == null || usuario.TokenTrocaEmailExpira < DateTime.UtcNow)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(usuario.NovoEmailPendente))
        {
            usuario.Email = usuario.NovoEmailPendente;
            usuario.NovoEmailPendente = null;
        }

        usuario.TokenTrocaEmail = null;
        usuario.TokenTrocaEmailExpira = null;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> CancelarTrocaEmailAsync(Guid usuarioId)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            return false;
        }

        usuario.NovoEmailPendente = null;
        usuario.TokenTrocaEmail = null;
        usuario.TokenTrocaEmailExpira = null;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static UsuarioDto MapearUsuarioParaDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Nome = usuario.Nome,
            Sobrenome = usuario.Sobrenome,
            Cpf = usuario.Cpf,
            Telefone = usuario.Telefone,
            Avatar = usuario.Avatar,
            Tipo = usuario.Tipo.ToString(),
            Status = usuario.Status.ToString(),
            EmailVerificado = usuario.EmailVerificado,
            CriadoEm = usuario.CriadoEm,
            AtualizadoEm = usuario.AtualizadoEm,
            PerfilPaciente = usuario.PerfilPaciente != null ? new PerfilPacienteDto
            {
                Id = usuario.PerfilPaciente.Id,
                Cns = usuario.PerfilPaciente.Cns,
                NomeSocial = usuario.PerfilPaciente.NomeSocial,
                Sexo = usuario.PerfilPaciente.Sexo,
                DataNascimento = usuario.PerfilPaciente.DataNascimento?.ToString("yyyy-MM-dd"),
                NomeMae = usuario.PerfilPaciente.NomeMae,
                NomePai = usuario.PerfilPaciente.NomePai,
                Nacionalidade = usuario.PerfilPaciente.Nacionalidade,
                Cep = usuario.PerfilPaciente.Cep,
                Endereco = usuario.PerfilPaciente.Endereco,
                Cidade = usuario.PerfilPaciente.Cidade,
                Estado = usuario.PerfilPaciente.Estado
            } : null,
            PerfilProfissional = usuario.PerfilProfissional != null ? new PerfilProfissionalDto
            {
                Id = usuario.PerfilProfissional.Id,
                Crm = usuario.PerfilProfissional.Crm,
                Cbo = usuario.PerfilProfissional.Cbo,
                EspecialidadeId = usuario.PerfilProfissional.EspecialidadeId,
                NomeEspecialidade = usuario.PerfilProfissional.Especialidade?.Nome,
                Sexo = usuario.PerfilProfissional.Sexo,
                DataNascimento = usuario.PerfilProfissional.DataNascimento?.ToString("yyyy-MM-dd"),
                Nacionalidade = usuario.PerfilProfissional.Nacionalidade,
                Cep = usuario.PerfilProfissional.Cep,
                Endereco = usuario.PerfilProfissional.Endereco,
                Cidade = usuario.PerfilProfissional.Cidade,
                Estado = usuario.PerfilProfissional.Estado
            } : null
        };
    }
}
