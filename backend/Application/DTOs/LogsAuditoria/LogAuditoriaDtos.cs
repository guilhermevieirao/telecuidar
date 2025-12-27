namespace Application.DTOs.LogsAuditoria;

/// <summary>
/// DTO de log de auditoria para retorno
/// </summary>
public class LogAuditoriaDto
{
    public Guid Id { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? NomeUsuario { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? Entidade { get; set; }
    public Guid? EntidadeId { get; set; }
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CriadoEm { get; set; }
}

/// <summary>
/// DTO para criar log de auditoria
/// </summary>
public class CriarLogAuditoriaDto
{
    public Guid? UsuarioId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? Entidade { get; set; }
    public Guid? EntidadeId { get; set; }
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// DTO para listagem paginada de logs de auditoria
/// </summary>
public class LogsAuditoriaPaginadosDto
{
    public List<LogAuditoriaDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para filtros de busca de logs
/// </summary>
public class FiltrosLogAuditoriaDto
{
    public Guid? UsuarioId { get; set; }
    public string? Acao { get; set; }
    public string? Entidade { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}
