using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Agenda de atendimento do profissional
/// Relacionamento N:1 com Usuario (Profissional)
/// </summary>
public class Agenda : EntidadeBase
{
    public Guid ProfissionalId { get; set; }
    
    /// <summary>
    /// JSON com configuração global de horários
    /// Estrutura: { intervalo, duracaoConsulta, intervaloEntreConsultas, etc. }
    /// </summary>
    public string ConfiguracaoGlobalJson { get; set; } = "{}";
    
    /// <summary>
    /// JSON com configuração por dia da semana
    /// Estrutura: Array de { diaSemana, ativo, horarios: [{inicio, fim}] }
    /// </summary>
    public string ConfiguracaoDiasJson { get; set; } = "[]";
    
    public DateTime DataInicioVigencia { get; set; }
    public DateTime? DataFimVigencia { get; set; }
    
    // Aliases para compatibilidade
    public DateTime DataInicioValidade { get => DataInicioVigencia; set => DataInicioVigencia = value; }
    public DateTime? DataFimValidade { get => DataFimVigencia; set => DataFimVigencia = value; }
    
    public bool Ativa { get; set; } = true;
    public StatusEspecialidade? Status { get; set; }
    
    // Navigation Property
    public Usuario Profissional { get; set; } = null!;
}
