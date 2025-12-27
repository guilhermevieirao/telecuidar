using Application.DTOs.Certificados;

namespace Application.Interfaces;

/// <summary>
/// Serviço de armazenamento de certificados digitais
/// </summary>
public interface ICertificadoService
{
    /// <summary>
    /// Obtém todos os certificados de um usuário
    /// </summary>
    Task<List<CertificadoDto>> ObterCertificadosPorUsuarioAsync(Guid usuarioId);

    /// <summary>
    /// Obtém um certificado por ID
    /// </summary>
    Task<CertificadoDto?> ObterCertificadoPorIdAsync(Guid id);

    /// <summary>
    /// Salva um novo certificado
    /// </summary>
    Task<CertificadoDto> SalvarCertificadoAsync(Guid usuarioId, Stream arquivoCertificado, string senha, string apelido);

    /// <summary>
    /// Deleta um certificado
    /// </summary>
    Task<bool> DeletarCertificadoAsync(Guid id, Guid usuarioId);

    /// <summary>
    /// Valida um certificado (verifica validade e senha)
    /// </summary>
    Task<bool> ValidarCertificadoAsync(Guid id, string senha);

    /// <summary>
    /// Assina um documento usando um certificado
    /// </summary>
    Task<byte[]?> AssinarDocumentoAsync(Guid certificadoId, string senha, byte[] documento);
}
