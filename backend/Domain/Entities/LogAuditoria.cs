using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Log de auditoria para rastreamento de ações no sistema
/// </summary>
public class LogAuditoria : EntidadeBase
{
    public Guid? UsuarioId { get; set; }
    
    public string Acao { get; set; } = string.Empty; // create, update, delete, login, etc.
    public string TipoEntidade { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string? EntidadeId { get; set; }
    public string? ValoresAntigos { get; set; } // JSON
    public string? ValoresNovos { get; set; } // JSON
    public string? DadosAntigos { get; set; } // JSON
    public string? DadosNovos { get; set; } // JSON
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
    
    // Navigation Property
    public Usuario? Usuario { get; set; }
}
