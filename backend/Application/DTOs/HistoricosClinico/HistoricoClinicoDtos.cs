namespace Application.DTOs.HistoricosClinico;

/// <summary>
/// DTO de histórico clínico para retorno
/// </summary>
public class HistoricoClinicoDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public string? NomePaciente { get; set; }
    
    // Dados estruturados (usados pelo serviço)
    public List<AlergiaDto>? Alergias { get; set; }
    public List<MedicamentoEmUsoDto>? MedicamentosEmUso { get; set; }
    public List<string>? DoencasCronicas { get; set; }
    public List<CirurgiaAnteriorDto>? CirurgiasAnteriores { get; set; }
    public List<HistoricoFamiliarDto>? HistoricoFamiliar { get; set; }
    public List<VacinacaoDto>? Vacinacoes { get; set; }
    public HabitosSociaisDto? HabitosSociais { get; set; }
    public string? TipoSanguineo { get; set; }
    public string? Observacoes { get; set; }
    
    // Antecedentes (legado)
    public string? AntecedentesPessoais { get; set; }
    public string? AntecedentesFamiliares { get; set; }
    public string? AlergiasMedicamentos { get; set; }
    public string? AlergiasAlimentos { get; set; }
    public string? OutrasAlergias { get; set; }
    
    // Condições crônicas (legado)
    public string? Hipertensao { get; set; }
    public string? Diabetes { get; set; }
    public string? OutrasCondicoes { get; set; }
    
    // Cirurgias e internações (legado)
    public string? InternacoesAnteriores { get; set; }
    
    // Medicamentos de uso contínuo (legado)
    public string? MedicamentosUsoContinuo { get; set; }
    
    // Hábitos (legado)
    public string? Tabagismo { get; set; }
    public string? Etilismo { get; set; }
    public string? AtividadeFisica { get; set; }
    public string? Alimentacao { get; set; }
    
    // Observações gerais (legado)
    public string? ObservacoesGerais { get; set; }
    
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criar ou atualizar histórico clínico
/// </summary>
public class SalvarHistoricoClinicoDto
{
    // Antecedentes
    public string? AntecedentesPessoais { get; set; }
    public string? AntecedentesFamiliares { get; set; }
    public string? AlergiasMedicamentos { get; set; }
    public string? AlergiasAlimentos { get; set; }
    public string? OutrasAlergias { get; set; }
    
    // Condições crônicas
    public string? Hipertensao { get; set; }
    public string? Diabetes { get; set; }
    public string? OutrasCondicoes { get; set; }
    
    // Cirurgias e internações
    public string? CirurgiasAnteriores { get; set; }
    public string? InternacoesAnteriores { get; set; }
    
    // Medicamentos de uso contínuo
    public string? MedicamentosUsoContinuo { get; set; }
    
    // Hábitos
    public string? Tabagismo { get; set; }
    public string? Etilismo { get; set; }
    public string? AtividadeFisica { get; set; }
    public string? Alimentacao { get; set; }
    
    // Observações gerais
    public string? ObservacoesGerais { get; set; }
}

/// <summary>
/// DTO para timeline do paciente
/// </summary>
public class TimelinePacienteDto
{
    public Guid PacienteId { get; set; }
    public string NomePaciente { get; set; } = string.Empty;
    public List<EventoTimelineDto> Eventos { get; set; } = new();
}

/// <summary>
/// DTO para evento da timeline
/// </summary>
public class EventoTimelineDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // Consulta, Prescricao, Atestado, Exame
    public DateTime Data { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Profissional { get; set; }
    public string? Especialidade { get; set; }
}

/// <summary>
/// DTO para atualizar histórico clínico
/// </summary>
public class AtualizarHistoricoClinicoDto
{
    public List<AlergiaDto>? Alergias { get; set; }
    public List<MedicamentoEmUsoDto>? MedicamentosEmUso { get; set; }
    public List<string>? DoencasCronicas { get; set; }
    public List<CirurgiaAnteriorDto>? CirurgiasAnteriores { get; set; }
    public List<HistoricoFamiliarDto>? HistoricoFamiliar { get; set; }
    public List<VacinacaoDto>? Vacinacoes { get; set; }
    public HabitosSociaisDto? HabitosSociais { get; set; }
    public string? TipoSanguineo { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para alergia
/// </summary>
public class AlergiaDto
{
    public string Substancia { get; set; } = string.Empty;
    public string? Gravidade { get; set; }
    public string? Reacao { get; set; }
    public DateTime? DataDiagnostico { get; set; }
}

/// <summary>
/// DTO para medicamento em uso
/// </summary>
public class MedicamentoEmUsoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Dosagem { get; set; }
    public string? Frequencia { get; set; }
    public DateTime? DataInicio { get; set; }
    public string? Observacao { get; set; }
}

/// <summary>
/// DTO para cirurgia anterior
/// </summary>
public class CirurgiaAnteriorDto
{
    public string Procedimento { get; set; } = string.Empty;
    public DateTime? Data { get; set; }
    public string? Hospital { get; set; }
    public string? Observacao { get; set; }
}

/// <summary>
/// DTO para histórico familiar
/// </summary>
public class HistoricoFamiliarDto
{
    public string Parentesco { get; set; } = string.Empty;
    public string Condicao { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}

/// <summary>
/// DTO para vacinação
/// </summary>
public class VacinacaoDto
{
    public string Vacina { get; set; } = string.Empty;
    public DateTime? Data { get; set; }
    public string? Dose { get; set; }
    public string? Lote { get; set; }
}

/// <summary>
/// DTO para hábitos sociais
/// </summary>
public class HabitosSociaisDto
{
    public bool Tabagismo { get; set; }
    public string? FrequenciaTabagismo { get; set; }
    public bool Etilismo { get; set; }
    public string? FrequenciaEtilismo { get; set; }
    public bool AtividadeFisica { get; set; }
    public string? FrequenciaAtividadeFisica { get; set; }
    public string? Dieta { get; set; }
}

/// <summary>
/// DTO para histórico de consultas
/// </summary>
public class HistoricoConsultasDto
{
    public List<ConsultaHistoricoDto> Consultas { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para consulta no histórico
/// </summary>
public class ConsultaHistoricoDto
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public string? Horario { get; set; }
    public string? NomeProfissional { get; set; }
    public string? Especialidade { get; set; }
    public string? Subjetivo { get; set; }
    public string? Objetivo { get; set; }
    public string? Avaliacao { get; set; }
    public string? Plano { get; set; }
    public bool TemPrescricao { get; set; }
    public bool TemAtestado { get; set; }
}

/// <summary>
/// DTO para resumo clínico
/// </summary>
public class ResumoClinicoDto
{
    public Guid PacienteId { get; set; }
    public string? TipoSanguineo { get; set; }
    public int QuantidadeAlergias { get; set; }
    public List<string> AlergiasGraves { get; set; } = new();
    public int QuantidadeMedicamentosEmUso { get; set; }
    public List<string> DoencasCronicas { get; set; } = new();
    public int TotalConsultasRealizadas { get; set; }
    public List<UltimaConsultaDto> UltimasConsultas { get; set; } = new();
}

/// <summary>
/// DTO para última consulta
/// </summary>
public class UltimaConsultaDto
{
    public DateTime Data { get; set; }
    public string? Especialidade { get; set; }
}
