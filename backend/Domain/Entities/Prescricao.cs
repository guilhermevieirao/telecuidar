using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Prescrição médica (receita)
/// Relacionamento N:1 com Consulta
/// </summary>
public class Prescricao : EntidadeBase
{
    public Guid ConsultaId { get; set; }
    public Guid ProfissionalId { get; set; }
    public Guid PacienteId { get; set; }
    
    /// <summary>
    /// Tipo de prescrição (normal, controlada, etc.)
    /// </summary>
    public string? Tipo { get; set; }
    
    /// <summary>
    /// Observações gerais da prescrição
    /// </summary>
    public string? Observacoes { get; set; }
    
    /// <summary>
    /// Validade em dias da prescrição
    /// </summary>
    public int? ValidadeEmDias { get; set; }
    
    /// <summary>
    /// Indica se foi assinado digitalmente
    /// </summary>
    public bool AssinadoDigitalmente { get; set; }
    
    /// <summary>
    /// ID do certificado usado para assinar
    /// </summary>
    public Guid? CertificadoId { get; set; }
    
    /// <summary>
    /// Data da assinatura digital
    /// </summary>
    public DateTime? DataAssinatura { get; set; }
    
    /// <summary>
    /// JSON com array de itens da prescrição
    /// Estrutura: [{ medicamento, codigoAnvisa, dosagem, frequencia, periodo, posologia, observacoes }]
    /// </summary>
    public string ItensJson { get; set; } = "[]";
    
    // Assinatura Digital ICP-Brasil
    public string? AssinaturaDigital { get; set; } // Base64
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

/// <summary>
/// Item individual de uma prescrição
/// </summary>
public class ItemPrescricao
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Medicamento { get; set; } = string.Empty;
    public string? CodigoAnvisa { get; set; }
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Posologia { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
}
