using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Bloqueio de horário na agenda do profissional
/// Relacionamento N:1 com Usuario (Profissional)
/// </summary>
public class BloqueioAgenda : EntidadeBase
{
    public Guid ProfissionalId { get; set; }
    
    public TipoBloqueioAgenda Tipo { get; set; }
    
    // Para bloqueios de um dia único
    public DateTime? Data { get; set; }
    
    // Para bloqueios de período
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    
    public string Motivo { get; set; } = string.Empty;
    public StatusBloqueioAgenda Status { get; set; } = StatusBloqueioAgenda.Pendente;
    
    // Aprovação
    public Guid? AprovadoPorId { get; set; }
    public DateTime? AprovadoEm { get; set; }
    public string? MotivoRejeicao { get; set; }
    
    // Navigation Properties
    public Usuario Profissional { get; set; } = null!;
    public Usuario? AprovadoPor { get; set; }
}
