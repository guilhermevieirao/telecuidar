namespace Application.DTOs.Agendas;

/// <summary>
/// DTO de agenda para retorno
/// </summary>
public class AgendaDto
{
    public Guid Id { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? NomeProfissional { get; set; }
    public ConfiguracaoGlobalDto? ConfiguracaoGlobal { get; set; }
    public List<ConfiguracaoDiaDto>? ConfiguracaoDias { get; set; }
    public string DataInicioVigencia { get; set; } = string.Empty;
    public string? DataFimVigencia { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para configuração global da agenda
/// </summary>
public class ConfiguracaoGlobalDto
{
    public int DuracaoConsulta { get; set; } = 30;
    public int IntervaloEntreConsultas { get; set; } = 0;
    public int? TempoIntervalo { get; set; }
    public string? HorarioInicioIntervalo { get; set; }
    public string? HorarioFimIntervalo { get; set; }
}

/// <summary>
/// DTO para configuração de dia da agenda
/// </summary>
public class ConfiguracaoDiaDto
{
    public int DiaSemana { get; set; } // 0 = Domingo, 6 = Sábado
    public bool Ativo { get; set; }
    public List<FaixaHorarioDto>? Horarios { get; set; }
}

/// <summary>
/// DTO para faixa de horário
/// </summary>
public class FaixaHorarioDto
{
    public string Inicio { get; set; } = string.Empty;
    public string Fim { get; set; } = string.Empty;
}

/// <summary>
/// DTO para criar agenda
/// </summary>
public class CriarAgendaDto
{
    public Guid ProfissionalId { get; set; }
    public ConfiguracaoGlobalDto ConfiguracaoGlobal { get; set; } = new();
    public List<ConfiguracaoDiaDto> ConfiguracaoDias { get; set; } = new();
    public string DataInicioVigencia { get; set; } = string.Empty;
    public string? DataFimVigencia { get; set; }
    public string Status { get; set; } = "Ativa";
}

/// <summary>
/// DTO para atualizar agenda
/// </summary>
public class AtualizarAgendaDto
{
    public ConfiguracaoGlobalDto? ConfiguracaoGlobal { get; set; }
    public List<ConfiguracaoDiaDto>? ConfiguracaoDias { get; set; }
    public string? DataInicioVigencia { get; set; }
    public string? DataFimVigencia { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO para listagem paginada de agendas
/// </summary>
public class AgendasPaginadasDto
{
    public List<AgendaDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
