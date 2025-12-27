using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Anamnese - Histórico clínico coletado pelo profissional
/// Relacionamento 1:1 com Consulta
/// </summary>
public class Anamnese : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    
    /// <summary>
    /// Queixa principal do paciente
    /// </summary>
    public string? QueixaPrincipal { get; set; }
    
    /// <summary>
    /// História da doença atual
    /// </summary>
    public string? HistoriaDoencaAtual { get; set; }
    
    /// <summary>
    /// História patológica pregressa (doenças anteriores)
    /// </summary>
    public string? HistoriaPatologicaPregressa { get; set; }
    
    /// <summary>
    /// História familiar (doenças na família)
    /// </summary>
    public string? HistoriaFamiliar { get; set; }
    
    /// <summary>
    /// Hábitos de vida (alimentação, exercícios, sono, etc.)
    /// </summary>
    public string? HabitosVida { get; set; }
    
    /// <summary>
    /// Revisão de sistemas (sintomas por sistema orgânico)
    /// </summary>
    public string? RevisaoSistemas { get; set; }
    
    /// <summary>
    /// Medicamentos em uso
    /// </summary>
    public string? MedicamentosEmUso { get; set; }
    
    /// <summary>
    /// Alergias conhecidas
    /// </summary>
    public string? Alergias { get; set; }
    
    /// <summary>
    /// Observações adicionais
    /// </summary>
    public string? Observacoes { get; set; }
    
    // Navigation Property
    public Consulta Consulta { get; set; } = null!;
}
