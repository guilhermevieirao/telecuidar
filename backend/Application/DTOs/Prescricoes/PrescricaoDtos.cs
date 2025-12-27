namespace Application.DTOs.Prescricoes;

/// <summary>
/// DTO de prescrição para retorno
/// </summary>
public class PrescricaoDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? NomeProfissional { get; set; }
    public string? CrmProfissional { get; set; }
    public Guid PacienteId { get; set; }
    public string? NomePaciente { get; set; }
    public string? CpfPaciente { get; set; }
    
    public string? Tipo { get; set; }
    public List<ItemPrescricaoDto>? Itens { get; set; } = new();
    public string? Observacoes { get; set; }
    public int ValidadeEmDias { get; set; }
    
    public bool Assinada { get; set; }
    public bool AssinadoDigitalmente { get; set; }
    public DateTime? AssinadoEm { get; set; }
    public string? SubjetoCertificado { get; set; }
    public string? CertificadoId { get; set; }
    public DateTime? DataAssinatura { get; set; }
    public string? HashDocumento { get; set; }
    public bool PdfDisponivel { get; set; }
    
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para item da prescrição
/// </summary>
public class ItemPrescricaoDto
{
    public string Id { get; set; } = string.Empty;
    public string Medicamento { get; set; } = string.Empty;
    public string? Nome { get; set; }
    public string? CodigoAnvisa { get; set; }
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Posologia { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para criar prescrição
/// </summary>
public class CriarPrescricaoDto
{
    public Guid ConsultaId { get; set; }
    public string? Tipo { get; set; }
    public List<ItemPrescricaoDto> Itens { get; set; } = new();
    public string? Observacoes { get; set; }
    public int ValidadeEmDias { get; set; } = 30;
}

/// <summary>
/// DTO para atualizar prescrição
/// </summary>
public class AtualizarPrescricaoDto
{
    public string? Tipo { get; set; }
    public List<ItemPrescricaoDto>? Itens { get; set; }
    public string? Observacoes { get; set; }
    public int? ValidadeEmDias { get; set; }
}

/// <summary>
/// DTO para assinar prescrição
/// </summary>
public class AssinarPrescricaoDto
{
    public string CertificadoPfxBase64 { get; set; } = string.Empty;
    public string SenhaCertificado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para assinar com certificado salvo
/// </summary>
public class AssinarComCertificadoSalvoDto
{
    public Guid CertificadoId { get; set; }
    public string? Senha { get; set; }
}

/// <summary>
/// DTO para listagem paginada de prescrições
/// </summary>
public class PrescricoesPaginadasDto
{
    public List<PrescricaoDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para filtros de prescrição
/// </summary>
public class FiltrosPrescricaoDto
{
    public Guid? ConsultaId { get; set; }
    public Guid? PacienteId { get; set; }
    public Guid? ProfissionalId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}
