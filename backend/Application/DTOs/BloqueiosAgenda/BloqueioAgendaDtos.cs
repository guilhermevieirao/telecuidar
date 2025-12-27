namespace Application.DTOs.BloqueiosAgenda;

/// <summary>
/// DTO de bloqueio de agenda para retorno
/// </summary>
public class BloqueioAgendaDto
{
    public Guid Id { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? NomeProfissional { get; set; }
    public string Tipo { get; set; } = string.Empty; // Unico, Periodo
    public string? Data { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? AprovadoPorId { get; set; }
    public string? NomeAprovador { get; set; }
    public DateTime? AprovadoEm { get; set; }
    public string? MotivoRejeicao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criar bloqueio de agenda
/// </summary>
public class CriarBloqueioAgendaDto
{
    public string Tipo { get; set; } = "Unico";
    public string? Data { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualizar bloqueio de agenda
/// </summary>
public class AtualizarBloqueioAgendaDto
{
    public string? Tipo { get; set; }
    public string? Data { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public string? Motivo { get; set; }
}

/// <summary>
/// DTO para aprovar bloqueio de agenda
/// </summary>
public class AprovarBloqueioAgendaDto
{
    // Pode conter observações futuras se necessário
}

/// <summary>
/// DTO para rejeitar bloqueio de agenda
/// </summary>
public class RejeitarBloqueioAgendaDto
{
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para listagem paginada de bloqueios de agenda
/// </summary>
public class BloqueiosAgendaPaginadosDto
{
    public List<BloqueioAgendaDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
