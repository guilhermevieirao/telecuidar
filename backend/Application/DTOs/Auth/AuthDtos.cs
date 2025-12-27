namespace Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de login
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool LembrarMe { get; set; } = false;
}

/// <summary>
/// DTO para resposta de login
/// </summary>
public class LoginResponseDto
{
    public Application.DTOs.Usuarios.UsuarioDto Usuario { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para requisição de registro
/// </summary>
public class RegistroRequestDto
{
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Senha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
    public bool AceitarTermos { get; set; }
    public string? TokenConvite { get; set; }
}

/// <summary>
/// DTO para resposta de registro
/// </summary>
public class RegistroResponseDto
{
    public Application.DTOs.Usuarios.UsuarioDto Usuario { get; set; } = null!;
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para requisição de esqueci a senha
/// </summary>
public class EsqueciSenhaRequestDto
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de esqueci a senha
/// </summary>
public class EsqueciSenhaResponseDto
{
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para requisição de reset de senha
/// </summary>
public class ResetSenhaRequestDto
{
    public string Token { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de reset de senha
/// </summary>
public class ResetSenhaResponseDto
{
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para requisição de verificação de email
/// </summary>
public class VerificarEmailRequestDto
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de verificação de email
/// </summary>
public class VerificarEmailResponseDto
{
    public string Mensagem { get; set; } = string.Empty;
    public Application.DTOs.Usuarios.UsuarioDto? Usuario { get; set; }
}

/// <summary>
/// DTO para requisição de refresh token
/// </summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de refresh token
/// </summary>
public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para requisição de troca de email
/// </summary>
public class TrocarEmailRequestDto
{
    public string NovoEmail { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de troca de email
/// </summary>
public class TrocarEmailResponseDto
{
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para verificar troca de email
/// </summary>
public class VerificarTrocaEmailRequestDto
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de token (usado em renovar token)
/// </summary>
public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}

/// <summary>
/// DTO para renovar token
/// </summary>
public class RenovarTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para logout
/// </summary>
public class LogoutRequestDto
{
    public string? RefreshToken { get; set; }
}

/// <summary>
/// DTO para redefinir senha
/// </summary>
public class RedefinirSenhaRequestDto
{
    public string Token { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para reenviar verificação de email
/// </summary>
public class ReenviarVerificacaoRequestDto
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO para alterar senha
/// </summary>
public class AlterarSenhaRequestDto
{
    public string SenhaAtual { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
}
