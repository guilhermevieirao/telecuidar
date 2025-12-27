using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Mensagem com anexo no chat da teleconsulta
/// Relacionamento N:1 com Consulta
/// </summary>
public class AnexoChat : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    public Guid RemetenteId { get; set; }
    public Guid EnviadoPorId { get; set; }
    
    public string? Mensagem { get; set; }
    public string? NomeArquivo { get; set; }
    public string NomeOriginal { get; set; } = string.Empty;
    public string? CaminhoArquivo { get; set; }
    public string? TipoArquivo { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long? TamanhoArquivo { get; set; }
    public long TamanhoBytes { get; set; }
    public DateTime EnviadoEm { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public Consulta Consulta { get; set; } = null!;
    public Usuario Remetente { get; set; } = null!;
    public Usuario EnviadoPor { get; set; } = null!;
}
