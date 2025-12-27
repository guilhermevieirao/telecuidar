using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Especialidade médica com campos customizáveis
/// </summary>
public class Especialidade : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public StatusEspecialidade Status { get; set; } = StatusEspecialidade.Ativa;
    
    /// <summary>
    /// JSON com campos customizados da especialidade
    /// Estrutura: Array de { nome, tipo, opcoes, obrigatorio, etc. }
    /// Mantido como JSON pois é dinâmico e variável por especialidade
    /// </summary>
    public string? CamposPersonalizadosJson { get; set; }
    
    // Navigation Properties
    public ICollection<PerfilProfissional> Profissionais { get; set; } = new List<PerfilProfissional>();
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}
