namespace Application.DTOs.Jitsi;

/// <summary>
/// DTO para configuração do Jitsi
/// </summary>
public class JitsiConfigDto
{
    public string Domain { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public JitsiUserDto User { get; set; } = null!;
    public string Jwt { get; set; } = string.Empty;
}

/// <summary>
/// DTO para usuário do Jitsi
/// </summary>
public class JitsiUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool Moderator { get; set; }
}

/// <summary>
/// DTO para criação de sala Jitsi
/// </summary>
public class CriarSalaJitsiDto
{
    public Guid ConsultaId { get; set; }
}

/// <summary>
/// DTO para resposta de criação de sala
/// </summary>
public class CriarSalaJitsiResponseDto
{
    public string RoomName { get; set; } = string.Empty;
    public string Jwt { get; set; } = string.Empty;
    public string JoinUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO para entrar na sala Jitsi
/// </summary>
public class EntrarSalaJitsiDto
{
    public Guid ConsultaId { get; set; }
}

/// <summary>
/// DTO para resposta de entrar na sala
/// </summary>
public class EntrarSalaJitsiResponseDto
{
    public JitsiConfigDto Config { get; set; } = null!;
    public string JoinUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO para sala Jitsi
/// </summary>
public class SalaJitsiDto
{
    public string NomeSala { get; set; } = string.Empty;
    public string UrlCompleta { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public Guid ConsultaId { get; set; }
}

/// <summary>
/// DTO para token Jitsi
/// </summary>
public class TokenJitsiDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public string NomeSala { get; set; } = string.Empty;
    public string UrlCompleta { get; set; } = string.Empty;
}

/// <summary>
/// DTO para gerar token Jitsi
/// </summary>
public class GerarTokenJitsiDto
{
    public string NomeSala { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool Moderador { get; set; }
    public Dictionary<string, bool>? Features { get; set; }
}

/// <summary>
/// DTO para configuração Jitsi
/// </summary>
public class ConfiguracaoJitsiDto
{
    public string Domain { get; set; } = string.Empty;
    public Dictionary<string, object> InterfaceConfigOverwrite { get; set; } = new();
    public Dictionary<string, object> ConfigOverwrite { get; set; } = new();
}
