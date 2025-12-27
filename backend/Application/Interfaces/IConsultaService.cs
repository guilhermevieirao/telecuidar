using Application.DTOs.Consultas;

namespace Application.Interfaces;

/// <summary>
/// Serviço de consultas
/// </summary>
public interface IConsultaService
{
    Task<ConsultasPaginadasDto> ObterConsultasAsync(int pagina, int tamanhoPagina, string? busca, string? status, DateTime? dataInicio, DateTime? dataFim, Guid? pacienteId = null, Guid? profissionalId = null);
    Task<ConsultaDto?> ObterConsultaPorIdAsync(Guid id);
    Task<ConsultaDto> CriarConsultaAsync(CriarConsultaDto dto);
    Task<ConsultaDto?> AtualizarConsultaAsync(Guid id, AtualizarConsultaDto dto);
    Task<bool> CancelarConsultaAsync(Guid id);
    Task<bool> FinalizarConsultaAsync(Guid id);
    Task<bool> IniciarConsultaAsync(Guid id);
    
    // Horários disponíveis
    Task<List<SlotDisponivelDto>> ObterHorariosDisponiveisAsync(Guid profissionalId, DateTime data);
    
    // Dados clínicos
    Task<PreConsultaDto?> ObterPreConsultaAsync(Guid consultaId);
    Task<PreConsultaDto> SalvarPreConsultaAsync(Guid consultaId, PreConsultaDto dto);
    
    Task<AnamneseDto?> ObterAnamneseAsync(Guid consultaId);
    Task<AnamneseDto> SalvarAnamneseAsync(Guid consultaId, AnamneseDto dto);
    
    Task<RegistroSoapDto?> ObterSoapAsync(Guid consultaId);
    Task<RegistroSoapDto> SalvarSoapAsync(Guid consultaId, RegistroSoapDto dto);
    
    Task<DadosBiometricosDto?> ObterDadosBiometricosAsync(Guid consultaId);
    Task<DadosBiometricosDto> SalvarDadosBiometricosAsync(Guid consultaId, DadosBiometricosDto dto);
}
