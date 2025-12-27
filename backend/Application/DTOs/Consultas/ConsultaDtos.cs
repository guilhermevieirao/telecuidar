namespace Application.DTOs.Consultas;

/// <summary>
/// DTO de consulta para retorno
/// </summary>
public class ConsultaDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public string? NomePaciente { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? NomeProfissional { get; set; }
    public Guid EspecialidadeId { get; set; }
    public string? NomeEspecialidade { get; set; }
    
    public DateTime Data { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string? HorarioFim { get; set; }
    
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public string? LinkVideo { get; set; }
    
    public string? CamposEspecialidadeJson { get; set; }
    
    public string? ResumoIA { get; set; }
    public DateTime? ResumoIAGeradoEm { get; set; }
    public string? HipoteseDiagnosticaIA { get; set; }
    public DateTime? HipoteseDiagnosticaIAGeradaEm { get; set; }
    
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    
    // Dados clínicos relacionados
    public PreConsultaDto? PreConsulta { get; set; }
    public AnamneseDto? Anamnese { get; set; }
    public RegistroSoapDto? Soap { get; set; }
    public DadosBiometricosDto? DadosBiometricos { get; set; }
    public List<AnexoDto>? Anexos { get; set; }
}

/// <summary>
/// DTO para criar consulta
/// </summary>
public class CriarConsultaDto
{
    public Guid PacienteId { get; set; }
    public Guid ProfissionalId { get; set; }
    public Guid EspecialidadeId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? Observacao { get; set; }
}

/// <summary>
/// DTO para atualizar consulta
/// </summary>
public class AtualizarConsultaDto
{
    public string? Data { get; set; }
    public string? Horario { get; set; }
    public string? HorarioFim { get; set; }
    public string? Tipo { get; set; }
    public string? Status { get; set; }
    public string? Observacao { get; set; }
    public string? CamposEspecialidadeJson { get; set; }
}

/// <summary>
/// DTO para listagem paginada de consultas
/// </summary>
public class ConsultasPaginadasDto
{
    public List<ConsultaDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para pré-consulta
/// </summary>
public class PreConsultaDto
{
    public Guid? Id { get; set; }
    public Guid ConsultaId { get; set; }
    
    // Informações Pessoais
    public string? NomeCompleto { get; set; }
    public string? DataNascimento { get; set; }
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    
    // Histórico Médico
    public string? CondicoesCronicas { get; set; }
    public string? Medicamentos { get; set; }
    public string? Alergias { get; set; }
    public string? Cirurgias { get; set; }
    public string? ObservacoesHistorico { get; set; }
    
    // Hábitos de Vida
    public string? Tabagismo { get; set; }
    public string? ConsumoAlcool { get; set; }
    public string? AtividadeFisica { get; set; }
    public string? ObservacoesHabitos { get; set; }
    
    // Sinais Vitais
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? Temperatura { get; set; }
    public string? SaturacaoOxigenio { get; set; }
    public string? ObservacoesSinaisVitais { get; set; }
    
    // Sintomas
    public string? SintomasPrincipais { get; set; }
    public string? InicioSintomas { get; set; }
    public int? IntensidadeDor { get; set; }
    public string? ObservacoesSintomas { get; set; }
    
    public string? ObservacoesAdicionais { get; set; }
}

/// <summary>
/// DTO para anamnese
/// </summary>
public class AnamneseDto
{
    public Guid? Id { get; set; }
    public Guid ConsultaId { get; set; }
    
    public string? QueixaPrincipal { get; set; }
    public string? HistoriaDoencaAtual { get; set; }
    public string? HistoriaPatologicaPregressa { get; set; }
    public string? HistoriaFamiliar { get; set; }
    public string? HabitosVida { get; set; }
    public string? RevisaoSistemas { get; set; }
    public string? MedicamentosEmUso { get; set; }
    public string? Alergias { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para registro SOAP
/// </summary>
public class RegistroSoapDto
{
    public Guid? Id { get; set; }
    public Guid ConsultaId { get; set; }
    
    public string? Subjetivo { get; set; }
    public string? Objetivo { get; set; }
    public string? Avaliacao { get; set; }
    public string? Plano { get; set; }
}

/// <summary>
/// DTO para dados biométricos
/// </summary>
public class DadosBiometricosDto
{
    public Guid? Id { get; set; }
    public Guid ConsultaId { get; set; }
    
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? FrequenciaRespiratoria { get; set; }
    public string? Temperatura { get; set; }
    public string? SaturacaoOxigenio { get; set; }
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    public string? Imc { get; set; }
    public string? CircunferenciaAbdominal { get; set; }
    public string? Glicemia { get; set; }
    public string? TipoGlicemia { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para anexo
/// </summary>
public class AnexoDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long TamanhoArquivo { get; set; }
    public DateTime CriadoEm { get; set; }
}

/// <summary>
/// DTO para criar anexo
/// </summary>
public class CriarAnexoDto
{
    public Guid ConsultaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string ConteudoBase64 { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para anexo do chat
/// </summary>
public class AnexoChatDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public Guid RemetenteId { get; set; }
    public string? NomeRemetente { get; set; }
    public string? Mensagem { get; set; }
    public string? NomeArquivo { get; set; }
    public string? UrlArquivo { get; set; }
    public string? TipoArquivo { get; set; }
    public long? TamanhoArquivo { get; set; }
    public DateTime EnviadoEm { get; set; }
}

/// <summary>
/// DTO para criar anexo do chat
/// </summary>
public class CriarAnexoChatDto
{
    public Guid ConsultaId { get; set; }
    public string? Mensagem { get; set; }
    public string? NomeArquivo { get; set; }
    public string? ConteudoBase64 { get; set; }
    public string? TipoArquivo { get; set; }
}

/// <summary>
/// DTO para slot de horário disponível
/// </summary>
public class SlotDisponivelDto
{
    public string Horario { get; set; } = string.Empty;
    public string HorarioFim { get; set; } = string.Empty;
    public bool Disponivel { get; set; }
}

/// <summary>
/// DTO para filtros de consulta
/// </summary>
public class FiltrosConsultaDto
{
    public Guid? PacienteId { get; set; }
    public Guid? ProfissionalId { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? Status { get; set; }
    public string? Tipo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}

/// <summary>
/// DTO para consulta detalhada
/// </summary>
public class ConsultaDetalhadaDto : ConsultaDto
{
    public string? NomePacienteCompleto { get; set; }
    public string? NomeProfissionalCompleto { get; set; }
    public string? CrmProfissional { get; set; }
}

/// <summary>
/// DTO para agendar consulta
/// </summary>
public class AgendarConsultaDto
{
    public Guid PacienteId { get; set; }
    public Guid ProfissionalId { get; set; }
    public Guid EspecialidadeId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? Observacao { get; set; }
}

/// <summary>
/// DTO para cancelar consulta
/// </summary>
public class CancelarConsultaDto
{
    public string? Motivo { get; set; }
}

/// <summary>
/// DTO para salvar pré-consulta
/// </summary>
public class SalvarPreConsultaDto
{
    // Informações Pessoais
    public string? NomeCompleto { get; set; }
    public string? DataNascimento { get; set; }
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    
    // Histórico Médico
    public string? CondicoesCronicas { get; set; }
    public string? Medicamentos { get; set; }
    public string? Alergias { get; set; }
    public string? Cirurgias { get; set; }
    public string? ObservacoesHistorico { get; set; }
    
    // Hábitos de Vida
    public string? Tabagismo { get; set; }
    public string? ConsumoAlcool { get; set; }
    public string? AtividadeFisica { get; set; }
    public string? ObservacoesHabitos { get; set; }
    
    // Sinais Vitais
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? Temperatura { get; set; }
    public string? SaturacaoOxigenio { get; set; }
    public string? ObservacoesSinaisVitais { get; set; }
    
    // Sintomas
    public string? SintomasPrincipais { get; set; }
    public string? InicioSintomas { get; set; }
    public int? IntensidadeDor { get; set; }
    public string? ObservacoesSintomas { get; set; }
    
    public string? ObservacoesAdicionais { get; set; }
}

/// <summary>
/// DTO para salvar anamnese
/// </summary>
public class SalvarAnamneseDto
{
    public string? QueixaPrincipal { get; set; }
    public string? HistoriaDoencaAtual { get; set; }
    public string? HistoriaPatologicaPregressa { get; set; }
    public string? HistoriaFamiliar { get; set; }
    public string? HabitosVida { get; set; }
    public string? RevisaoSistemas { get; set; }
    public string? MedicamentosEmUso { get; set; }
    public string? Alergias { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para salvar registro SOAP
/// </summary>
public class SalvarRegistroSoapDto
{
    public string? Subjetivo { get; set; }
    public string? Objetivo { get; set; }
    public string? Avaliacao { get; set; }
    public string? Plano { get; set; }
}

/// <summary>
/// DTO para salvar dados biométricos
/// </summary>
public class SalvarDadosBiometricosDto
{
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? FrequenciaRespiratoria { get; set; }
    public string? Temperatura { get; set; }
    public string? SaturacaoOxigenio { get; set; }
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    public string? Imc { get; set; }
    public string? CircunferenciaAbdominal { get; set; }
    public string? Glicemia { get; set; }
    public string? TipoGlicemia { get; set; }
    public string? Observacoes { get; set; }
}
