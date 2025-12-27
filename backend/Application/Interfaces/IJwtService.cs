namespace Application.Interfaces;

/// <summary>
/// Serviço de JWT
/// </summary>
public interface IJwtService
{
    string GerarAccessToken(Guid usuarioId, string email, string nome, string tipo);
    string GerarRefreshToken();
    (bool Valido, Guid? UsuarioId, string? Email, string? Tipo) ValidarToken(string token);
    bool ValidarRefreshToken(string refreshToken, Guid usuarioId);
    Task SalvarRefreshTokenAsync(Guid usuarioId, string refreshToken, TimeSpan expiracao);
    Task RevogarRefreshTokenAsync(Guid usuarioId);
    Task<bool> RefreshTokenExisteAsync(Guid usuarioId, string refreshToken);
}
