using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Registro SOAP - Formato padronizado de registro médico
/// S = Subjetivo, O = Objetivo, A = Avaliação, P = Plano
/// Relacionamento 1:1 com Consulta
/// </summary>
public class RegistroSoap : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    
    /// <summary>
    /// S - Subjetivo: Informações relatadas pelo paciente
    /// Sintomas, queixas, percepções do paciente
    /// </summary>
    public string? Subjetivo { get; set; }
    
    /// <summary>
    /// O - Objetivo: Dados observados/mensurados pelo profissional
    /// Exame físico, sinais vitais, resultados de exames
    /// </summary>
    public string? Objetivo { get; set; }
    
    /// <summary>
    /// A - Avaliação: Análise e interpretação do profissional
    /// Diagnósticos, hipóteses diagnósticas
    /// </summary>
    public string? Avaliacao { get; set; }
    
    /// <summary>
    /// P - Plano: Conduta terapêutica
    /// Tratamentos, medicações, exames solicitados, retorno
    /// </summary>
    public string? Plano { get; set; }
    
    // Navigation Property
    public Consulta Consulta { get; set; } = null!;
}
