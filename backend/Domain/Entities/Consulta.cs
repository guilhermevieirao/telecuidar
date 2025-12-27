using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Consulta médica - entidade central do sistema
/// Relaciona paciente, profissional e todos os dados clínicos
/// </summary>
public class Consulta : EntidadeBase
{
    public Guid PacienteId { get; set; }
    public Guid ProfissionalId { get; set; }
    public Guid EspecialidadeId { get; set; }
    
    public DateTime Data { get; set; }
    public TimeSpan Horario { get; set; }
    public TimeSpan? HorarioFim { get; set; }
    
    // Aliases para compatibilidade
    public TimeSpan HoraInicio { get => Horario; set => Horario = value; }
    public TimeSpan? HoraFim { get => HorarioFim; set => HorarioFim = value; }
    
    public TipoConsulta Tipo { get; set; }
    public StatusConsulta Status { get; set; }
    
    public string? Observacao { get; set; }
    public string? LinkVideo { get; set; }
    
    /// <summary>
    /// JSON com valores dos campos personalizados da especialidade
    /// Mantido como JSON pois campos são dinâmicos
    /// </summary>
    public string? CamposEspecialidadeJson { get; set; }
    
    // Dados gerados por IA
    public string? ResumoIA { get; set; }
    public DateTime? ResumoIAGeradoEm { get; set; }
    public string? HipoteseDiagnosticaIA { get; set; }
    public DateTime? HipoteseDiagnosticaIAGeradaEm { get; set; }
    
    // Navigation Properties
    public Usuario Paciente { get; set; } = null!;
    public Usuario Profissional { get; set; } = null!;
    public Especialidade Especialidade { get; set; } = null!;
    
    // Relacionamentos 1:1 com dados clínicos
    public PreConsulta? PreConsulta { get; set; }
    public Anamnese? Anamnese { get; set; }
    public RegistroSoap? Soap { get; set; }
    public RegistroSoap? RegistroSoap { get => Soap; set => Soap = value; }
    public DadosBiometricos? DadosBiometricos { get; set; }
    
    // Relacionamentos 1:N
    public ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();
    public ICollection<AnexoChat> AnexosChat { get; set; } = new List<AnexoChat>();
    public ICollection<Prescricao> Prescricoes { get; set; } = new List<Prescricao>();
    public ICollection<AtestadoMedico> Atestados { get; set; } = new List<AtestadoMedico>();
}
