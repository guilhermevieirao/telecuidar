using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Histórico clínico do paciente - agregado de consultas anteriores
/// Para visualização em /consultas/{id}/detalhes
/// </summary>
public class HistoricoClinico : EntidadeBase
{
    public Guid PacienteId { get; set; }
    
    /// <summary>
    /// Condições crônicas conhecidas
    /// </summary>
    public string? CondicoesCronicas { get; set; }
    
    /// <summary>
    /// Alergias conhecidas
    /// </summary>
    public string? Alergias { get; set; }
    
    /// <summary>
    /// Medicamentos de uso contínuo
    /// </summary>
    public string? MedicamentosUsoContinuo { get; set; }
    
    /// <summary>
    /// Cirurgias anteriores
    /// </summary>
    public string? CirurgiasAnteriores { get; set; }
    
    /// <summary>
    /// Histórico familiar relevante
    /// </summary>
    public string? HistoricoFamiliar { get; set; }
    
    /// <summary>
    /// Observações gerais sobre o paciente
    /// </summary>
    public string? Observacoes { get; set; }
    
    /// <summary>
    /// Tipo sanguíneo do paciente
    /// </summary>
    public string? TipoSanguineo { get; set; }
    
    // Propriedades JSON para armazenar listas estruturadas
    public string? AlergiasJson { get; set; }
    public string? MedicamentosEmUsoJson { get; set; }
    public string? DoencasCronicasJson { get; set; }
    public string? CirurgiasAnterioresJson { get; set; }
    public string? HistoricoFamiliarJson { get; set; }
    public string? VacinacoesJson { get; set; }
    public string? HabitosSociaisJson { get; set; }
    
    // Navigation Property
    public Usuario Paciente { get; set; } = null!;
}
