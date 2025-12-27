namespace Application.DTOs.Convites;

/// <summary>
/// DTO de convite para retorno
/// </summary>
public class ConviteDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
    public Guid? EspecialidadeId { get; set; }
    public string? NomeEspecialidade { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public Guid CriadoPorId { get; set; }
    public string? NomeCriador { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AceitoEm { get; set; }
}

/// <summary>
/// DTO para criar convite
/// </summary>
public class CriarConviteDto
{
    public string? Email { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
    public Guid? EspecialidadeId { get; set; }
    public int? DiasValidade { get; set; }
}

/// <summary>
/// DTO para validar convite
/// </summary>
public class ValidarConviteDto
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de validação de convite
/// </summary>
public class ValidarConviteResponseDto
{
    public bool Valido { get; set; }
    public string? Mensagem { get; set; }
    public string? Email { get; set; }
    public string? TipoUsuario { get; set; }
    public Guid? EspecialidadeId { get; set; }
}

/// <summary>
/// DTO para listagem paginada de convites
/// </summary>
public class ConvitesPaginadosDto
{
    public List<ConviteDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
