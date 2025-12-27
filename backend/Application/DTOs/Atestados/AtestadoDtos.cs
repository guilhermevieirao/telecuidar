namespace Application.DTOs.Atestados;

/// <summary>
/// DTO de atestado médico para retorno
/// </summary>
public class AtestadoMedicoDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? NomeProfissional { get; set; }
    public string? CrmProfissional { get; set; }
    public Guid PacienteId { get; set; }
    public string? NomePaciente { get; set; }
    public string? CpfPaciente { get; set; }
    
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? DiasAfastamento { get; set; }
    public string? Cid { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    
    public bool Assinado { get; set; }
    public DateTime? AssinadoEm { get; set; }
    public string? SubjetoCertificado { get; set; }
    public string? HashDocumento { get; set; }
    public bool PdfDisponivel { get; set; }
    
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criar atestado médico
/// </summary>
public class CriarAtestadoMedicoDto
{
    public Guid ConsultaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public int? DiasAfastamento { get; set; }
    public string? Cid { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para atualizar atestado médico
/// </summary>
public class AtualizarAtestadoMedicoDto
{
    public string? Tipo { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public int? DiasAfastamento { get; set; }
    public string? Cid { get; set; }
    public string? Conteudo { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para assinar atestado médico
/// </summary>
public class AssinarAtestadoDto
{
    public string CertificadoPfxBase64 { get; set; } = string.Empty;
    public string SenhaCertificado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para listagem paginada de atestados
/// </summary>
public class AtestadosPaginadosDto
{
    public List<AtestadoDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para atestado médico (compatibilidade com serviço)
/// </summary>
public class AtestadoDto
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public string? NomePaciente { get; set; }
    public string? NomeProfissional { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public int DiasTotais { get; set; }
    public string? Conteudo { get; set; }
    public string? Cid { get; set; }
    public bool AssinadoDigitalmente { get; set; }
    public string? CertificadoId { get; set; }
    public DateTime? DataAssinatura { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criar atestado (compatibilidade com serviço)
/// </summary>
public class CriarAtestadoDto
{
    public Guid ConsultaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public int DiasTotais { get; set; }
    public string? Conteudo { get; set; }
    public string? Cid { get; set; }
}

/// <summary>
/// DTO para atualizar atestado (compatibilidade com serviço)
/// </summary>
public class AtualizarAtestadoDto
{
    public string? Tipo { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public int? DiasTotais { get; set; }
    public string? Conteudo { get; set; }
    public string? Cid { get; set; }
}

/// <summary>
/// DTO para filtros de atestados
/// </summary>
public class FiltrosAtestadoDto
{
    public Guid? ConsultaId { get; set; }
    public Guid? PacienteId { get; set; }
    public Guid? ProfissionalId { get; set; }
    public string? Tipo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}
