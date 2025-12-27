using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Entidade base de usuário com campos comuns a todos os tipos
/// Os campos específicos estão em PerfilPaciente e PerfilProfissional
/// </summary>
public class Usuario : EntidadeBase
{
    // Dados de autenticação
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    
    // Dados básicos de identificação
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Avatar { get; set; }
    
    // Controle de acesso
    public TipoUsuario Tipo { get; set; }
    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;
    
    // Verificação de email
    public bool EmailVerificado { get; set; } = false;
    public string? TokenVerificacaoEmail { get; set; }
    public DateTime? ExpiracaoTokenVerificacaoEmail { get; set; }
    
    // Alias para compatibilidade
    public DateTime? TokenVerificacaoEmailExpira { get => ExpiracaoTokenVerificacaoEmail; set => ExpiracaoTokenVerificacaoEmail = value; }
    
    // Mudança de email pendente
    public string? EmailPendente { get; set; }
    public string? TokenEmailPendente { get; set; }
    public DateTime? ExpiracaoTokenEmailPendente { get; set; }
    
    // Aliases para troca de email
    public string? NovoEmailPendente { get => EmailPendente; set => EmailPendente = value; }
    public string? TokenTrocaEmail { get => TokenEmailPendente; set => TokenEmailPendente = value; }
    public DateTime? TokenTrocaEmailExpira { get => ExpiracaoTokenEmailPendente; set => ExpiracaoTokenEmailPendente = value; }
    
    // Reset de senha
    public string? TokenResetSenha { get; set; }
    public DateTime? ExpiracaoTokenResetSenha { get; set; }
    
    // Aliases para compatibilidade
    public string? TokenRedefinicaoSenha { get => TokenResetSenha; set => TokenResetSenha = value; }
    public DateTime? TokenRedefinicaoSenhaExpira { get => ExpiracaoTokenResetSenha; set => ExpiracaoTokenResetSenha = value; }
    
    // Refresh Token para autenticação
    public string? RefreshToken { get; set; }
    public DateTime? ExpiracaoRefreshToken { get; set; }
    public DateTime? RefreshTokenExpira { get; set; }
    
    // ============================================
    // Navigation Properties - Perfis específicos
    // ============================================
    public PerfilPaciente? PerfilPaciente { get; set; }
    public PerfilProfissional? PerfilProfissional { get; set; }
    
    // Navigation Properties - Relacionamentos
    public ICollection<Consulta> ConsultasComoPaciente { get; set; } = new List<Consulta>();
    public ICollection<Consulta> ConsultasComoProfissional { get; set; } = new List<Consulta>();
    public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    public ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();
    public ICollection<LogAuditoria> LogsAuditoria { get; set; } = new List<LogAuditoria>();
    public ICollection<BloqueioAgenda> BloqueiosAgenda { get; set; } = new List<BloqueioAgenda>();
}
