using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Dados da pré-consulta preenchidos pelo paciente
/// Relacionamento 1:1 com Consulta
/// </summary>
public class PreConsulta : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    
    // ============================================
    // Informações Pessoais
    // ============================================
    public string? NomeCompleto { get; set; }
    public string? DataNascimento { get; set; }
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    
    // ============================================
    // Histórico Médico
    // ============================================
    public string? CondicoesCronicas { get; set; }
    public string? Medicamentos { get; set; }
    public string? Alergias { get; set; }
    public string? Cirurgias { get; set; }
    public string? ObservacoesHistorico { get; set; }
    
    // ============================================
    // Hábitos de Vida
    // ============================================
    public string? Tabagismo { get; set; } // sim, nao, ex-fumante
    public string? ConsumoAlcool { get; set; } // nenhum, social, frequente
    public string? AtividadeFisica { get; set; } // nenhuma, leve, moderada, intensa
    public string? ObservacoesHabitos { get; set; }
    
    // ============================================
    // Sinais Vitais (aferidos pelo paciente)
    // ============================================
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? Temperatura { get; set; }
    public string? SaturacaoOxigenio { get; set; }
    public string? ObservacoesSinaisVitais { get; set; }
    
    // ============================================
    // Sintomas Atuais
    // ============================================
    public string? SintomasPrincipais { get; set; }
    public string? InicioSintomas { get; set; }
    public int? IntensidadeDor { get; set; } // 0-10
    public string? ObservacoesSintomas { get; set; }
    
    // Observações adicionais gerais
    public string? ObservacoesAdicionais { get; set; }
    
    // Navigation Property
    public Consulta Consulta { get; set; } = null!;
}
