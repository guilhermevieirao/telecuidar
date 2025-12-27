namespace Application.DTOs.Comum;

/// <summary>
/// DTO para resposta genérica
/// </summary>
public class RespostaDto
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
}

/// <summary>
/// DTO para resposta genérica com dados
/// </summary>
public class RespostaDto<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
}

/// <summary>
/// DTO para erro de validação
/// </summary>
public class ErroValidacaoDto
{
    public string Campo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de erro
/// </summary>
public class RespostaErroDto
{
    public bool Sucesso { get; set; } = false;
    public string Mensagem { get; set; } = string.Empty;
    public List<ErroValidacaoDto>? Erros { get; set; }
    public string? CodigoErro { get; set; }
}

/// <summary>
/// DTO para paginação
/// </summary>
public class PaginacaoDto
{
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
    public string? OrdenarPor { get; set; }
    public bool Descendente { get; set; } = false;
}

/// <summary>
/// DTO para resultado paginado
/// </summary>
public class ResultadoPaginadoDto<T>
{
    public List<T> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemProxima => Pagina < TotalPaginas;
    public bool TemAnterior => Pagina > 1;
}

/// <summary>
/// DTO para opção de select (dropdown)
/// </summary>
public class OpcaoSelectDto
{
    public string Valor { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public bool? Desabilitado { get; set; }
}

/// <summary>
/// DTO para opção de select com ID
/// </summary>
public class OpcaoSelectGuidDto
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public bool? Desabilitado { get; set; }
}

/// <summary>
/// DTO para upload de arquivo
/// </summary>
public class UploadArquivoDto
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string ConteudoBase64 { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long Tamanho { get; set; }
}

/// <summary>
/// DTO para resposta de upload
/// </summary>
public class UploadArquivoResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public long Tamanho { get; set; }
}
