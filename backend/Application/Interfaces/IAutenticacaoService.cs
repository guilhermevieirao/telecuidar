using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Serviço de autenticação
/// </summary>
public interface IAutenticacaoService
{
    Task<LoginResponseDto?> LoginAsync(string email, string senha, bool lembrarMe);
    Task<Usuario> RegistrarAsync(string nome, string sobrenome, string email, string cpf, string? telefone, string senha, string? tokenConvite = null);
    Task<RefreshTokenResponseDto?> RenovarTokenAsync(string refreshToken);
    Task<bool> EsqueciSenhaAsync(string email);
    Task<bool> RedefinirSenhaAsync(string token, string novaSenha);
    Task<bool> VerificarEmailAsync(string token);
    Task<Usuario?> VerificarEmailComUsuarioAsync(string token);
    Task<bool> ReenviarEmailVerificacaoAsync(string email);
    Task<bool> AlterarSenhaAsync(Guid usuarioId, string senhaAtual, string novaSenha);
    Task<bool> EmailDisponivelAsync(string email);
    Task<bool> CpfDisponivelAsync(string cpf);
    Task<bool> TelefoneDisponivelAsync(string telefone);
    
    // Mudança de email
    Task<bool> SolicitarTrocaEmailAsync(Guid usuarioId, string novoEmail);
    Task<Usuario?> VerificarTrocaEmailAsync(string token);
    Task<bool> CancelarTrocaEmailAsync(Guid usuarioId);
}
