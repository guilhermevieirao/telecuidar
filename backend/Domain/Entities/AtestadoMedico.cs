using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Atestado médico
/// Relacionamento N:1 com Consulta
/// </summary>
public class AtestadoMedico : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    public Guid ProfissionalId { get; set; }
    public Guid PacienteId { get; set; }
    
    public TipoAtestado Tipo { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    
    // Para atestados de afastamento
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? DiasAfastamento { get; set; }
    public int DiasTotais { get; set; }
    public bool AssinadoDigitalmente { get; set; }
    
    /// <summary>
    /// ID do certificado usado para assinar
    /// </summary>
    public Guid? CertificadoId { get; set; }
    
    /// <summary>
    /// Data da assinatura digital
    /// </summary>
    public DateTime? DataAssinatura { get; set; }
    
    public string? Cid { get; set; } // Código CID
    public string Conteudo { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    
    // Assinatura Digital ICP-Brasil
    public string? AssinaturaDigital { get; set; }
    public string? ImpressaoDigitalCertificado { get; set; }
    public string? SubjetoCertificado { get; set; }
    public DateTime? AssinadoEm { get; set; }
    public string? HashDocumento { get; set; }
    public string? PdfAssinadoBase64 { get; set; }
    
    // Navigation Properties
    public Consulta Consulta { get; set; } = null!;
    public Usuario Profissional { get; set; } = null!;
    public Usuario Paciente { get; set; } = null!;
}
