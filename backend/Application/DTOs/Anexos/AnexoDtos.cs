namespace Application.DTOs.Anexos;

/// <summary>
/// DTO de anexo de consulta
/// </summary>
public class AnexoDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public Guid EnviadoPorId { get; set; }
    public string? NomeEnviador { get; set; }
    public string NomeOriginal { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public string? UrlDownload { get; set; }
    public DateTime CriadoEm { get; set; }
}

/// <summary>
/// DTO de anexo de chat
/// </summary>
public class AnexoChatDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public Guid EnviadoPorId { get; set; }
    public string? NomeEnviador { get; set; }
    public string NomeOriginal { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public string? UrlDownload { get; set; }
    public DateTime CriadoEm { get; set; }
}

/// <summary>
/// DTO para upload de anexo
/// </summary>
public class UploadAnexoDto
{
    public Guid ConsultaId { get; set; }
    // O arquivo é tratado via IFormFile no controller
}
