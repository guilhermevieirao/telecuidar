namespace Application.DTOs.AI;

/// <summary>
/// DTO para gerar resumo IA
/// </summary>
public class GerarResumoIADto
{
    public Guid ConsultaId { get; set; }
}

/// <summary>
/// DTO para resposta de resumo IA
/// </summary>
public class ResumoIAResponseDto
{
    public string Resumo { get; set; } = string.Empty;
    public DateTime GeradoEm { get; set; }
}

/// <summary>
/// DTO para gerar hipótese diagnóstica IA
/// </summary>
public class GerarHipoteseDiagnosticaIADto
{
    public Guid ConsultaId { get; set; }
}

/// <summary>
/// DTO para resposta de hipótese diagnóstica IA
/// </summary>
public class HipoteseDiagnosticaIAResponseDto
{
    public string HipoteseDiagnostica { get; set; } = string.Empty;
    public DateTime GeradoEm { get; set; }
}

/// <summary>
/// DTO para transcrição de áudio
/// </summary>
public class TranscreverAudioDto
{
    public string AudioBase64 { get; set; } = string.Empty;
    public string Formato { get; set; } = "mp3";
}

/// <summary>
/// DTO para resposta de transcrição
/// </summary>
public class TranscricaoResponseDto
{
    public string Texto { get; set; } = string.Empty;
    public double Duracao { get; set; }
}

/// <summary>
/// DTO para análise de texto
/// </summary>
public class AnalisarTextoDto
{
    public string Texto { get; set; } = string.Empty;
    public string TipoAnalise { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de análise
/// </summary>
public class AnaliseTextoResponseDto
{
    public string Resultado { get; set; } = string.Empty;
    public Dictionary<string, string>? MetaDados { get; set; }
}

/// <summary>
/// DTO para resposta de IA
/// </summary>
public class RespostaIADto
{
    public bool Sucesso { get; set; }
    public string? Conteudo { get; set; }
    public string? Erro { get; set; }
    public string? Tipo { get; set; }
    public int TokensUtilizados { get; set; }
}

/// <summary>
/// DTO para dados de consulta para IA
/// </summary>
public class DadosConsultaIADto
{
    public string? QueixaPrincipal { get; set; }
    public string? HistoriaDoencaAtual { get; set; }
    public List<string>? Sintomas { get; set; }
    public string? ExameFisico { get; set; }
    public SoapIADto? Soap { get; set; }
    public DadosBiometricosIADto? Biometricos { get; set; }
}

/// <summary>
/// DTO para dados SOAP
/// </summary>
public class SoapIADto
{
    public string? Subjetivo { get; set; }
    public string? Objetivo { get; set; }
    public string? Avaliacao { get; set; }
    public string? Plano { get; set; }
}

/// <summary>
/// DTO para dados biométricos
/// </summary>
public class DadosBiometricosIADto
{
    public int? PressaoSistolica { get; set; }
    public int? PressaoDiastolica { get; set; }
    public int? FrequenciaCardiaca { get; set; }
    public decimal? Temperatura { get; set; }
    public int? SaturacaoOxigenio { get; set; }
    public int? GlicemiaCapilar { get; set; }
}
