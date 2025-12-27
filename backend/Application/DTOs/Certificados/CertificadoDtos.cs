namespace Application.DTOs.Certificados;

/// <summary>
/// DTO de certificado salvo para retorno
/// </summary>
public class CertificadoSalvoDto
{
    public Guid Id { get; set; }
    public Guid ProfissionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string SubjetoCertificado { get; set; } = string.Empty;
    public DateTime ValidoAte { get; set; }
    public bool SenhaSalva { get; set; }
    public bool Valido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para salvar certificado
/// </summary>
public class SalvarCertificadoDto
{
    public string Nome { get; set; } = string.Empty;
    public string CertificadoPfxBase64 { get; set; } = string.Empty;
    public string SenhaCertificado { get; set; } = string.Empty;
    public bool SalvarSenha { get; set; }
}

/// <summary>
/// DTO para atualizar certificado
/// </summary>
public class AtualizarCertificadoDto
{
    public string? Nome { get; set; }
    public string? CertificadoPfxBase64 { get; set; }
    public string? SenhaCertificado { get; set; }
    public bool? SalvarSenha { get; set; }
}

/// <summary>
/// DTO para listagem de certificados
/// </summary>
public class CertificadosPaginadosDto
{
    public List<CertificadoSalvoDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para validar certificado
/// </summary>
public class ValidarCertificadoDto
{
    public string CertificadoPfxBase64 { get; set; } = string.Empty;
    public string SenhaCertificado { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resultado de validação de certificado
/// </summary>
public class ValidacaoCertificadoDto
{
    public bool Valido { get; set; }
    public string? NomeProprietario { get; set; }
    public string? CpfCnpj { get; set; }
    public DateTime? DataValidade { get; set; }
    public string? Thumbprint { get; set; }
    public string? Mensagem { get; set; }
}

/// <summary>
/// DTO para resposta de validação de certificado
/// </summary>
public class ValidarCertificadoResponseDto
{
    public bool Valido { get; set; }
    public string? SubjetoCertificado { get; set; }
    public DateTime? ValidoAte { get; set; }
    public string? Mensagem { get; set; }
}

/// <summary>
/// DTO de certificado para retorno (compatibilidade com serviço)
/// </summary>
public class CertificadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Apelido { get; set; } = string.Empty;
    public string? NomeProprietario { get; set; }
    public string? CpfCnpj { get; set; }
    public DateTime DataValidade { get; set; }
    public string? Thumbprint { get; set; }
    public bool Valido { get; set; }
    public DateTime CriadoEm { get; set; }
}
