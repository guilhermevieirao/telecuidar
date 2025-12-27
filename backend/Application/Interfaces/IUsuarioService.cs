using Application.DTOs.Usuarios;

namespace Application.Interfaces;

/// <summary>
/// Serviço de usuários
/// </summary>
public interface IUsuarioService
{
    Task<UsuariosPaginadosDto> ObterUsuariosAsync(int pagina, int tamanhoPagina, string? busca, string? tipo, string? status, Guid? especialidadeId = null);
    Task<UsuarioDto?> ObterUsuarioPorIdAsync(Guid id);
    Task<UsuarioDto?> ObterUsuarioPorEmailAsync(string email);
    Task<UsuarioDto?> ObterUsuarioPorCpfAsync(string cpf);
    Task<UsuarioDto?> ObterUsuarioPorTelefoneAsync(string telefone);
    Task<UsuarioDto> CriarUsuarioAsync(CriarUsuarioDto dto);
    Task<UsuarioDto?> AtualizarUsuarioAsync(Guid id, AtualizarUsuarioDto dto);
    Task<bool> DeletarUsuarioAsync(Guid id);
    
    // Perfis
    Task<PerfilPacienteDto?> ObterPerfilPacienteAsync(Guid usuarioId);
    Task<PerfilProfissionalDto?> ObterPerfilProfissionalAsync(Guid usuarioId);
    Task<PerfilPacienteDto> CriarOuAtualizarPerfilPacienteAsync(Guid usuarioId, CriarPerfilPacienteDto dto);
    Task<PerfilProfissionalDto> CriarOuAtualizarPerfilProfissionalAsync(Guid usuarioId, CriarPerfilProfissionalDto dto);
    
    // Avatar
    Task<string?> AtualizarAvatarAsync(Guid usuarioId, string avatarBase64);
    Task<bool> RemoverAvatarAsync(Guid usuarioId);
}
