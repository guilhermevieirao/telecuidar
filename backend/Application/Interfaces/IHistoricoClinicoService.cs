using Application.DTOs.HistoricosClinico;

namespace Application.Interfaces;

/// <summary>
/// Serviço de histórico clínico do paciente
/// </summary>
public interface IHistoricoClinicoService
{
    /// <summary>
    /// Obtém histórico clínico de um paciente
    /// </summary>
    Task<HistoricoClinicoDto?> ObterHistoricoPorPacienteAsync(Guid pacienteId);

    /// <summary>
    /// Cria ou atualiza histórico clínico de um paciente
    /// </summary>
    Task<HistoricoClinicoDto> CriarOuAtualizarHistoricoAsync(Guid pacienteId, AtualizarHistoricoClinicoDto dto);

    /// <summary>
    /// Obtém histórico de consultas de um paciente
    /// </summary>
    Task<HistoricoConsultasDto> ObterHistoricoConsultasAsync(Guid pacienteId, int pagina = 1, int tamanhoPagina = 10);

    /// <summary>
    /// Obtém medicamentos em uso de um paciente
    /// </summary>
    Task<List<MedicamentoEmUsoDto>> ObterMedicamentosEmUsoAsync(Guid pacienteId);

    /// <summary>
    /// Obtém alergias de um paciente
    /// </summary>
    Task<List<AlergiaDto>> ObterAlergiasAsync(Guid pacienteId);

    /// <summary>
    /// Adiciona uma alergia ao histórico do paciente
    /// </summary>
    Task<bool> AdicionarAlergiaAsync(Guid pacienteId, AlergiaDto alergia);

    /// <summary>
    /// Adiciona um medicamento em uso ao histórico do paciente
    /// </summary>
    Task<bool> AdicionarMedicamentoEmUsoAsync(Guid pacienteId, MedicamentoEmUsoDto medicamento);

    /// <summary>
    /// Obtém resumo clínico de um paciente
    /// </summary>
    Task<ResumoClinicoDto> ObterResumoClinicoAsync(Guid pacienteId);
}
