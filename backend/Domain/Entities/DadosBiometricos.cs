using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Dados biométricos/sinais vitais coletados durante a consulta
/// Relacionamento 1:1 com Consulta
/// </summary>
public class DadosBiometricos : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    
    // Sinais Vitais
    public string? PressaoArterial { get; set; } // Ex: "120/80"
    public string? FrequenciaCardiaca { get; set; } // bpm
    public string? FrequenciaRespiratoria { get; set; } // rpm
    public string? Temperatura { get; set; } // °C
    public string? SaturacaoOxigenio { get; set; } // %
    
    // Medidas Antropométricas
    public string? Peso { get; set; } // kg
    public string? Altura { get; set; } // cm
    public string? Imc { get; set; } // calculado
    public string? CircunferenciaAbdominal { get; set; } // cm
    
    // Glicemia
    public string? Glicemia { get; set; } // mg/dL
    public string? TipoGlicemia { get; set; } // Jejum, Pós-prandial, Casual
    
    // Outros
    public string? Observacoes { get; set; }
    
    // Navigation Property
    public Consulta Consulta { get; set; } = null!;
}
