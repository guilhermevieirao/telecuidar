namespace Application.DTOs.Usuarios;

/// <summary>
/// DTO de usuário para retorno
/// </summary>
public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Avatar { get; set; }
    public string Tipo { get; set; } = string.Empty; // Paciente, Profissional, Administrador
    public string Status { get; set; } = string.Empty;
    public bool EmailVerificado { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    
    // Perfis específicos
    public PerfilPacienteDto? PerfilPaciente { get; set; }
    public PerfilProfissionalDto? PerfilProfissional { get; set; }
}

/// <summary>
/// DTO para perfil de paciente
/// </summary>
public class PerfilPacienteDto
{
    public Guid? Id { get; set; }
    public string? Cns { get; set; }
    public string? NomeSocial { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? NomeMae { get; set; }
    public string? NomePai { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

/// <summary>
/// DTO para perfil de profissional
/// </summary>
public class PerfilProfissionalDto
{
    public Guid? Id { get; set; }
    public string? Crm { get; set; }
    public string? Cbo { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? NomeEspecialidade { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

/// <summary>
/// DTO para criar usuário
/// </summary>
public class CriarUsuarioDto
{
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Senha { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    
    // Perfis
    public CriarPerfilPacienteDto? PerfilPaciente { get; set; }
    public CriarPerfilProfissionalDto? PerfilProfissional { get; set; }
}

public class CriarPerfilPacienteDto
{
    public string? Cns { get; set; }
    public string? NomeSocial { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? NomeMae { get; set; }
    public string? NomePai { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class CriarPerfilProfissionalDto
{
    public string? Crm { get; set; }
    public string? Cbo { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

/// <summary>
/// DTO para atualizar usuário
/// </summary>
public class AtualizarUsuarioDto
{
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Tipo { get; set; }
    public string? Status { get; set; }
    
    public AtualizarPerfilPacienteDto? PerfilPaciente { get; set; }
    public AtualizarPerfilProfissionalDto? PerfilProfissional { get; set; }
}

public class AtualizarPerfilPacienteDto
{
    public string? Cns { get; set; }
    public string? NomeSocial { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? NomeMae { get; set; }
    public string? NomePai { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class AtualizarPerfilProfissionalDto
{
    public string? Crm { get; set; }
    public string? Cbo { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? Sexo { get; set; }
    public string? DataNascimento { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

/// <summary>
/// DTO para listagem paginada de usuários
/// </summary>
public class UsuariosPaginadosDto
{
    public List<UsuarioDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para atualizar senha
/// </summary>
public class AtualizarSenhaDto
{
    public string SenhaAtual { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualizar avatar
/// </summary>
public class AtualizarAvatarDto
{
    public string Avatar { get; set; } = string.Empty; // Base64 ou URL
}
