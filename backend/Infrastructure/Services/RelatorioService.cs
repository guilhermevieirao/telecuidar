using Application.DTOs.Relatorios;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de geração de relatórios
/// </summary>
public class RelatorioService : IRelatorioService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(ApplicationDbContext context, ILogger<RelatorioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RelatorioDashboardDto> GerarRelatorioDashboardAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var inicio = dataInicio ?? DateTime.UtcNow.AddMonths(-1);
        var fim = dataFim ?? DateTime.UtcNow;

        var consultas = await _context.Consultas
            .Where(c => c.Data >= inicio && c.Data <= fim)
            .ToListAsync();

        var consultasRealizadas = consultas.Count(c => c.Status == StatusConsulta.Realizada);
        var consultasCanceladas = consultas.Count(c => c.Status == StatusConsulta.Cancelada);
        var consultasAgendadas = consultas.Count(c => c.Status == StatusConsulta.Agendada);

        var totalUsuarios = await _context.Usuarios.CountAsync();
        var totalPacientes = await _context.Usuarios.CountAsync(u => u.Tipo == TipoUsuario.Paciente);
        var totalProfissionais = await _context.Usuarios.CountAsync(u => u.Tipo == TipoUsuario.Profissional);

        return new RelatorioDashboardDto
        {
            PeriodoInicio = inicio,
            PeriodoFim = fim,
            TotalConsultas = consultas.Count,
            ConsultasRealizadas = consultasRealizadas,
            ConsultasCanceladas = consultasCanceladas,
            ConsultasAgendadas = consultasAgendadas,
            TaxaRealizacao = consultas.Count > 0 ? (decimal)consultasRealizadas / consultas.Count * 100 : 0,
            TotalUsuarios = totalUsuarios,
            TotalPacientes = totalPacientes,
            TotalProfissionais = totalProfissionais,
            NovasConsultasHoje = consultas.Count(c => c.CriadoEm.Date == DateTime.UtcNow.Date),
            NovosUsuariosHoje = await _context.Usuarios.CountAsync(u => u.CriadoEm.Date == DateTime.UtcNow.Date)
        };
    }

    public async Task<RelatorioConsultasDto> GerarRelatorioConsultasAsync(FiltrosRelatorioDto filtros)
    {
        var query = _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Profissional)
            .Include(c => c.Especialidade)
            .AsQueryable();

        if (filtros.DataInicio.HasValue)
            query = query.Where(c => c.Data >= filtros.DataInicio.Value);

        if (filtros.DataFim.HasValue)
            query = query.Where(c => c.Data <= filtros.DataFim.Value);

        if (filtros.ProfissionalId.HasValue)
            query = query.Where(c => c.ProfissionalId == filtros.ProfissionalId.Value);

        if (filtros.EspecialidadeId.HasValue)
            query = query.Where(c => c.EspecialidadeId == filtros.EspecialidadeId.Value);

        if (!string.IsNullOrEmpty(filtros.Status) && Enum.TryParse<StatusConsulta>(filtros.Status, true, out var status))
            query = query.Where(c => c.Status == status);

        var consultas = await query.OrderByDescending(c => c.Data).ToListAsync();

        // Agrupar por especialidade
        var porEspecialidade = consultas
            .GroupBy(c => c.Especialidade?.Nome ?? "Sem especialidade")
            .Select(g => new EstatisticaEspecialidadeDto
            {
                Especialidade = g.Key,
                TotalConsultas = g.Count(),
                Realizadas = g.Count(c => c.Status == StatusConsulta.Realizada),
                Canceladas = g.Count(c => c.Status == StatusConsulta.Cancelada)
            })
            .ToList();

        // Agrupar por profissional
        var porProfissional = consultas
            .GroupBy(c => new { c.ProfissionalId, Nome = c.Profissional != null ? $"{c.Profissional.Nome} {c.Profissional.Sobrenome}" : "Desconhecido" })
            .Select(g => new EstatisticaProfissionalDto
            {
                ProfissionalId = g.Key.ProfissionalId,
                NomeProfissional = g.Key.Nome,
                TotalConsultas = g.Count(),
                Realizadas = g.Count(c => c.Status == StatusConsulta.Realizada),
                Canceladas = g.Count(c => c.Status == StatusConsulta.Cancelada)
            })
            .OrderByDescending(p => p.TotalConsultas)
            .ToList();

        // Agrupar por dia
        var porDia = consultas
            .GroupBy(c => c.Data.Date)
            .Select(g => new EstatisticaDiariaDto
            {
                Data = g.Key,
                TotalConsultas = g.Count(),
                Realizadas = g.Count(c => c.Status == StatusConsulta.Realizada),
                Canceladas = g.Count(c => c.Status == StatusConsulta.Cancelada)
            })
            .OrderBy(d => d.Data)
            .ToList();

        return new RelatorioConsultasDto
        {
            PeriodoInicio = filtros.DataInicio,
            PeriodoFim = filtros.DataFim,
            TotalConsultas = consultas.Count,
            Realizadas = consultas.Count(c => c.Status == StatusConsulta.Realizada),
            Canceladas = consultas.Count(c => c.Status == StatusConsulta.Cancelada),
            Agendadas = consultas.Count(c => c.Status == StatusConsulta.Agendada),
            PorEspecialidade = porEspecialidade,
            PorProfissional = porProfissional,
            PorDia = porDia
        };
    }

    public async Task<RelatorioUsuariosDto> GerarRelatorioUsuariosAsync(FiltrosRelatorioDto filtros)
    {
        var query = _context.Usuarios.AsQueryable();

        if (filtros.DataInicio.HasValue)
            query = query.Where(u => u.CriadoEm >= filtros.DataInicio.Value);

        if (filtros.DataFim.HasValue)
            query = query.Where(u => u.CriadoEm <= filtros.DataFim.Value);

        var usuarios = await query.ToListAsync();

        // Agrupar por tipo
        var porTipo = usuarios
            .GroupBy(u => u.Tipo)
            .Select(g => new EstatisticaTipoUsuarioDto
            {
                Tipo = g.Key.ToString(),
                Total = g.Count(),
                Ativos = g.Count(u => u.Status == StatusUsuario.Ativo),
                Inativos = g.Count(u => u.Status != StatusUsuario.Ativo)
            })
            .ToList();

        // Usuários por mês
        var porMes = usuarios
            .GroupBy(u => new { u.CriadoEm.Year, u.CriadoEm.Month })
            .Select(g => new EstatisticaMensalUsuariosDto
            {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                NovosUsuarios = g.Count()
            })
            .OrderBy(m => m.Ano).ThenBy(m => m.Mes)
            .ToList();

        return new RelatorioUsuariosDto
        {
            PeriodoInicio = filtros.DataInicio,
            PeriodoFim = filtros.DataFim,
            TotalUsuarios = usuarios.Count,
            UsuariosAtivos = usuarios.Count(u => u.Status == StatusUsuario.Ativo),
            UsuariosInativos = usuarios.Count(u => u.Status != StatusUsuario.Ativo),
            PorTipo = porTipo,
            PorMes = porMes
        };
    }

    public async Task<RelatorioFinanceiroDto> GerarRelatorioFinanceiroAsync(FiltrosRelatorioDto filtros)
    {
        var inicio = filtros.DataInicio ?? DateTime.UtcNow.AddMonths(-1);
        var fim = filtros.DataFim ?? DateTime.UtcNow;

        var consultas = await _context.Consultas
            .Include(c => c.Especialidade)
            .Where(c => c.Data >= inicio && c.Data <= fim && c.Status == StatusConsulta.Realizada)
            .ToListAsync();

        // Calcular receita baseada no valor das especialidades (se existir)
        decimal receitaTotal = 0;
        var porEspecialidade = new List<ReceitaEspecialidadeDto>();

        var agrupado = consultas
            .GroupBy(c => c.Especialidade)
            .ToList();

        foreach (var grupo in agrupado)
        {
            // Aqui você pode adicionar lógica de precificação
            // Por enquanto, usando valor fictício
            var valorConsulta = 100m; // Valor base
            var receita = grupo.Count() * valorConsulta;
            receitaTotal += receita;

            porEspecialidade.Add(new ReceitaEspecialidadeDto
            {
                Especialidade = grupo.Key?.Nome ?? "Sem especialidade",
                QuantidadeConsultas = grupo.Count(),
                ReceitaTotal = receita
            });
        }

        return new RelatorioFinanceiroDto
        {
            PeriodoInicio = inicio,
            PeriodoFim = fim,
            TotalConsultasRealizadas = consultas.Count,
            ReceitaTotal = receitaTotal,
            TicketMedio = consultas.Count > 0 ? receitaTotal / consultas.Count : 0,
            PorEspecialidade = porEspecialidade
        };
    }

    public async Task<byte[]> ExportarRelatorioAsync(string tipoRelatorio, string formato, FiltrosRelatorioDto filtros)
    {
        // Implementação básica - em produção usar biblioteca como ClosedXML ou iTextSharp
        var sb = new System.Text.StringBuilder();

        switch (tipoRelatorio.ToLower())
        {
            case "dashboard":
                var dashboard = await GerarRelatorioDashboardAsync(filtros.DataInicio, filtros.DataFim);
                sb.AppendLine("Relatório Dashboard");
                sb.AppendLine($"Período: {dashboard.PeriodoInicio:dd/MM/yyyy} a {dashboard.PeriodoFim:dd/MM/yyyy}");
                sb.AppendLine($"Total Consultas: {dashboard.TotalConsultas}");
                sb.AppendLine($"Realizadas: {dashboard.ConsultasRealizadas}");
                sb.AppendLine($"Canceladas: {dashboard.ConsultasCanceladas}");
                sb.AppendLine($"Taxa de Realização: {dashboard.TaxaRealizacao:F2}%");
                break;

            case "consultas":
                var consultas = await GerarRelatorioConsultasAsync(filtros);
                sb.AppendLine("Relatório de Consultas");
                sb.AppendLine($"Total: {consultas.TotalConsultas}");
                sb.AppendLine($"Realizadas: {consultas.Realizadas}");
                sb.AppendLine($"Canceladas: {consultas.Canceladas}");
                break;

            case "usuarios":
                var usuarios = await GerarRelatorioUsuariosAsync(filtros);
                sb.AppendLine("Relatório de Usuários");
                sb.AppendLine($"Total: {usuarios.TotalUsuarios}");
                sb.AppendLine($"Ativos: {usuarios.UsuariosAtivos}");
                sb.AppendLine($"Inativos: {usuarios.UsuariosInativos}");
                break;

            default:
                sb.AppendLine("Tipo de relatório não suportado.");
                break;
        }

        return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
    }
}
