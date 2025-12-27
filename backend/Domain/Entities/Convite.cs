using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Convite para criação de usuário
/// Token enviado por email ou link genérico
/// </summary>
public class Convite : EntidadeBase
{
    public string? Email { get; set; } // Opcional para links genéricos
    public TipoUsuario TipoUsuario { get; set; }
    public Guid? EspecialidadeId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public StatusConvite Status { get; set; } = StatusConvite.Pendente;
    public DateTime ExpiraEm { get; set; }
    
    public Guid CriadoPorId { get; set; }
    public DateTime? AceitoEm { get; set; }
    
    // Navigation Properties
    public Usuario CriadoPor { get; set; } = null!;
    public Especialidade? Especialidade { get; set; }
}
