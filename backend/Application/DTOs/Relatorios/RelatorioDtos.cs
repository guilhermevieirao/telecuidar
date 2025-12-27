namespace Application.DTOs.Relatorios;

/// <summary>
/// DTO para parâmetros de relatório
/// </summary>
public class ParametrosRelatorioDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public Guid? ProfissionalId { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO para relatório de consultas
/// </summary>
public class RelatorioConsultasDto
{
    public DateTime? PeriodoInicio { get; set; }
    public DateTime? PeriodoFim { get; set; }
    public int TotalConsultas { get; set; }
    public int Realizadas { get; set; }
    public int Canceladas { get; set; }
    public int Agendadas { get; set; }
    public int PorRealizar { get; set; }
    public int NaoCompareceu { get; set; }
    public double TaxaCancelamento { get; set; }
    public double TempoMedioConsulta { get; set; }
    public List<EstatisticaEspecialidadeDto>? PorEspecialidade { get; set; }
    public List<EstatisticaProfissionalDto>? PorProfissional { get; set; }
    public List<EstatisticaDiariaDto>? PorDia { get; set; }
}

/// <summary>
/// DTO para consultas por dia
/// </summary>
public class ConsultasPorDiaDto
{
    public string Data { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Realizadas { get; set; }
    public int Canceladas { get; set; }
}

/// <summary>
/// DTO para consultas por especialidade
/// </summary>
public class ConsultasPorEspecialidadeDto
{
    public string Especialidade { get; set; } = string.Empty;
    public int Total { get; set; }
    public double Percentual { get; set; }
}

/// <summary>
/// DTO para relatório de profissionais
/// </summary>
public class RelatorioProfissionaisDto
{
    public int TotalProfissionais { get; set; }
    public int Ativos { get; set; }
    public int Inativos { get; set; }
    public List<ProfissionalResumoDto>? Profissionais { get; set; }
}

/// <summary>
/// DTO para resumo de profissional
/// </summary>
public class ProfissionalResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public int ConsultasRealizadas { get; set; }
    public int ConsultasCanceladas { get; set; }
    public double TempoMedio { get; set; }
    public double Avaliacao { get; set; }
}

/// <summary>
/// DTO para relatório de pacientes
/// </summary>
public class RelatorioPacientesDto
{
    public int TotalPacientes { get; set; }
    public int Ativos { get; set; }
    public int Inativos { get; set; }
    public int NovosNoMes { get; set; }
    public List<PacienteResumoDto>? Pacientes { get; set; }
}

/// <summary>
/// DTO para resumo de paciente
/// </summary>
public class PacienteResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TotalConsultas { get; set; }
    public DateTime? UltimaConsulta { get; set; }
}

/// <summary>
/// DTO para dashboard administrativo
/// </summary>
public class DashboardDto
{
    public int TotalUsuarios { get; set; }
    public int TotalPacientes { get; set; }
    public int TotalProfissionais { get; set; }
    public int ConsultasHoje { get; set; }
    public int ConsultasSemana { get; set; }
    public int ConsultasMes { get; set; }
    public List<ConsultasPorDiaDto>? ConsultasUltimosSete { get; set; }
    public List<ConsultasPorEspecialidadeDto>? PorEspecialidade { get; set; }
}

/// <summary>
/// DTO para relatório de dashboard
/// </summary>
public class RelatorioDashboardDto
{
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public int TotalConsultas { get; set; }
    public int ConsultasRealizadas { get; set; }
    public int ConsultasCanceladas { get; set; }
    public int ConsultasAgendadas { get; set; }
    public decimal TaxaRealizacao { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalPacientes { get; set; }
    public int TotalProfissionais { get; set; }
    public int NovasConsultasHoje { get; set; }
    public int NovosUsuariosHoje { get; set; }
}

/// <summary>
/// DTO para filtros de relatório
/// </summary>
public class FiltrosRelatorioDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public Guid? ProfissionalId { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO para estatísticas por especialidade
/// </summary>
public class EstatisticaEspecialidadeDto
{
    public string Especialidade { get; set; } = string.Empty;
    public int TotalConsultas { get; set; }
    public int Realizadas { get; set; }
    public int Canceladas { get; set; }
}

/// <summary>
/// DTO para estatísticas por profissional
/// </summary>
public class EstatisticaProfissionalDto
{
    public Guid ProfissionalId { get; set; }
    public string NomeProfissional { get; set; } = string.Empty;
    public int TotalConsultas { get; set; }
    public int Realizadas { get; set; }
    public int Canceladas { get; set; }
}

/// <summary>
/// DTO para estatísticas diárias
/// </summary>
public class EstatisticaDiariaDto
{
    public DateTime Data { get; set; }
    public int TotalConsultas { get; set; }
    public int Realizadas { get; set; }
    public int Canceladas { get; set; }
}

/// <summary>
/// DTO para relatório de usuários
/// </summary>
public class RelatorioUsuariosDto
{
    public DateTime? PeriodoInicio { get; set; }
    public DateTime? PeriodoFim { get; set; }
    public int TotalUsuarios { get; set; }
    public int UsuariosAtivos { get; set; }
    public int UsuariosInativos { get; set; }
    public List<EstatisticaTipoUsuarioDto>? PorTipo { get; set; }
    public List<EstatisticaMensalUsuariosDto>? PorMes { get; set; }
}

/// <summary>
/// DTO para estatísticas por tipo de usuário
/// </summary>
public class EstatisticaTipoUsuarioDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Ativos { get; set; }
    public int Inativos { get; set; }
}

/// <summary>
/// DTO para estatísticas mensais de usuários
/// </summary>
public class EstatisticaMensalUsuariosDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public int NovosUsuarios { get; set; }
}

/// <summary>
/// DTO para relatório financeiro
/// </summary>
public class RelatorioFinanceiroDto
{
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public int TotalConsultasRealizadas { get; set; }
    public decimal ReceitaTotal { get; set; }
    public decimal TicketMedio { get; set; }
    public List<ReceitaEspecialidadeDto>? PorEspecialidade { get; set; }
}

/// <summary>
/// DTO para receita por especialidade
/// </summary>
public class ReceitaEspecialidadeDto
{
    public string Especialidade { get; set; } = string.Empty;
    public int QuantidadeConsultas { get; set; }
    public decimal ReceitaTotal { get; set; }
}
