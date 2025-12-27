using Application.DTOs.Jitsi;

namespace Application.Interfaces;

/// <summary>
/// Serviço de integração com Jitsi Meet
/// </summary>
public interface IJitsiService
{
    /// <summary>
    /// Cria uma nova sala de videoconferência
    /// </summary>
    Task<SalaJitsiDto> CriarSalaAsync(CriarSalaJitsiDto dto);

    /// <summary>
    /// Gera token de acesso para uma sala
    /// </summary>
    Task<TokenJitsiDto> GerarTokenAsync(GerarTokenJitsiDto dto);

    /// <summary>
    /// Gera token para moderador da sala
    /// </summary>
    Task<TokenJitsiDto> GerarTokenModeradoAsync(string nomeSala, Guid usuarioId, string nomeUsuario, string emailUsuario);

    /// <summary>
    /// Gera token para participante da sala
    /// </summary>
    Task<TokenJitsiDto> GerarTokenParticipanteAsync(string nomeSala, Guid usuarioId, string nomeUsuario, string emailUsuario);

    /// <summary>
    /// Obtém configurações do Jitsi
    /// </summary>
    ConfiguracaoJitsiDto ObterConfiguracao();
}
