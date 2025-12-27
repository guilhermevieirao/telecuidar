using Application.DTOs.Atestados;

namespace Application.Interfaces;

/// <summary>
/// Serviço de atestados médicos
/// </summary>
public interface IAtestadoMedicoService
{
    /// <summary>
    /// Obtém atestados com paginação e filtros
    /// </summary>
    Task<AtestadosPaginadosDto> ObterAtestadosAsync(FiltrosAtestadoDto filtros);

    /// <summary>
    /// Obtém um atestado por ID
    /// </summary>
    Task<AtestadoDto?> ObterAtestadoPorIdAsync(Guid id);

    /// <summary>
    /// Obtém atestados de uma consulta específica
    /// </summary>
    Task<List<AtestadoDto>> ObterAtestadosPorConsultaAsync(Guid consultaId);

    /// <summary>
    /// Obtém atestados de um paciente
    /// </summary>
    Task<List<AtestadoDto>> ObterAtestadosPorPacienteAsync(Guid pacienteId);

    /// <summary>
    /// Cria um novo atestado
    /// </summary>
    Task<AtestadoDto> CriarAtestadoAsync(CriarAtestadoDto dto);

    /// <summary>
    /// Atualiza um atestado existente
    /// </summary>
    Task<AtestadoDto?> AtualizarAtestadoAsync(Guid id, AtualizarAtestadoDto dto);

    /// <summary>
    /// Deleta um atestado
    /// </summary>
    Task<bool> DeletarAtestadoAsync(Guid id);

    /// <summary>
    /// Assina digitalmente um atestado
    /// </summary>
    Task<bool> AssinarAtestadoAsync(Guid id, Guid profissionalId, string certificadoId);

    /// <summary>
    /// Gera PDF do atestado
    /// </summary>
    Task<byte[]> GerarPdfAtestadoAsync(Guid id);
}
