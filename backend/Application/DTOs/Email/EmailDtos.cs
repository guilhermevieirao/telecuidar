namespace Application.DTOs.Email;

/// <summary>
/// Configurações de email
/// </summary>
public class EmailSettings
{
    public bool Enabled { get; set; }
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
}

/// <summary>
/// DTO para enviar email
/// </summary>
public class EnviarEmailDto
{
    public string Destinatario { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public bool Html { get; set; } = true;
}

/// <summary>
/// DTO para email de verificação
/// </summary>
public class EnviarEmailVerificacaoDto
{
    public Guid UsuarioId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

/// <summary>
/// DTO para email de redefinição de senha
/// </summary>
public class EnviarEmailRedefinicaoSenhaDto
{
    public Guid UsuarioId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

/// <summary>
/// DTO para email de convite
/// </summary>
public class EnviarEmailConviteDto
{
    public string Email { get; set; } = string.Empty;
    public string TokenConvite { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = string.Empty;
    public string? NomeConvidador { get; set; }
}

/// <summary>
/// DTO para email de confirmação de consulta
/// </summary>
public class EnviarEmailConfirmacaoConsultaDto
{
    public string EmailPaciente { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty;
    public string NomeProfissional { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public string Horario { get; set; } = string.Empty;
}

/// <summary>
/// DTO para email de lembrete de consulta
/// </summary>
public class EnviarEmailLembreteConsultaDto
{
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string NomeProfissional { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string? LinkConsulta { get; set; }
}
