using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Anexo de arquivo associado a uma consulta
/// Relacionamento N:1 com Consulta
/// </summary>
public class Anexo : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    public Guid EnviadoPorId { get; set; }
    
    public string Titulo { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string NomeOriginal { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanhoArquivo { get; set; }
    public long TamanhoBytes { get; set; }
    
    // Navigation Properties
    public Consulta Consulta { get; set; } = null!;
    public Usuario EnviadoPor { get; set; } = null!;
}
