using Application.DTOs.Prescricoes;

namespace Application.Interfaces;

/// <summary>
/// Serviço de prescrições médicas
/// </summary>
public interface IPrescricaoService
{
    /// <summary>
    /// Obtém prescrições com paginação e filtros
    /// </summary>
    Task<PrescricoesPaginadasDto> ObterPrescricoesAsync(FiltrosPrescricaoDto filtros);

    /// <summary>
    /// Obtém uma prescrição por ID
    /// </summary>
    Task<PrescricaoDto?> ObterPrescricaoPorIdAsync(Guid id);

    /// <summary>
    /// Obtém prescrições de uma consulta específica
    /// </summary>
    Task<List<PrescricaoDto>> ObterPrescricoesPorConsultaAsync(Guid consultaId);

    /// <summary>
    /// Obtém prescrições de um paciente
    /// </summary>
    Task<List<PrescricaoDto>> ObterPrescricoesPorPacienteAsync(Guid pacienteId);

    /// <summary>
    /// Cria uma nova prescrição
    /// </summary>
    Task<PrescricaoDto> CriarPrescricaoAsync(CriarPrescricaoDto dto);

    /// <summary>
    /// Atualiza uma prescrição existente
    /// </summary>
    Task<PrescricaoDto?> AtualizarPrescricaoAsync(Guid id, AtualizarPrescricaoDto dto);

    /// <summary>
    /// Deleta uma prescrição
    /// </summary>
    Task<bool> DeletarPrescricaoAsync(Guid id);

    /// <summary>
    /// Assina digitalmente uma prescrição
    /// </summary>
    Task<bool> AssinarPrescricaoAsync(Guid id, Guid profissionalId, string certificadoId);

    /// <summary>
    /// Gera PDF da prescrição
    /// </summary>
    Task<byte[]> GerarPdfPrescricaoAsync(Guid id);
}
