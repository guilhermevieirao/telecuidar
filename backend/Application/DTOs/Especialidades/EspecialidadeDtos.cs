namespace Application.DTOs.Especialidades;

/// <summary>
/// DTO de especialidade para retorno
/// </summary>
public class EspecialidadeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CamposPersonalizadosJson { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criar especialidade
/// </summary>
public class CriarEspecialidadeDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? CamposPersonalizadosJson { get; set; }
}

/// <summary>
/// DTO para atualizar especialidade
/// </summary>
public class AtualizarEspecialidadeDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Status { get; set; }
    public string? CamposPersonalizadosJson { get; set; }
}

/// <summary>
/// DTO para listagem paginada de especialidades
/// </summary>
public class EspecialidadesPaginadasDto
{
    public List<EspecialidadeDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para campo personalizado da especialidade
/// </summary>
public class CampoPersonalizadoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // text, number, select, checkbox, etc.
    public string? Label { get; set; }
    public bool Obrigatorio { get; set; }
    public List<string>? Opcoes { get; set; }
    public string? ValorPadrao { get; set; }
}
