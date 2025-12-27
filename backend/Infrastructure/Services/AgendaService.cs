using Application.DTOs.Agendas;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de agenda
/// </summary>
public class AgendaService : IAgendaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AgendaService> _logger;

    public AgendaService(ApplicationDbContext context, ILogger<AgendaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AgendasPaginadasDto> ObterAgendasAsync(int pagina, int tamanhoPagina, Guid? profissionalId, string? status)
    {
        var query = _context.Agendas
            .Include(a => a.Profissional)
            .AsQueryable();

        if (profissionalId.HasValue)
        {
            query = query.Where(a => a.ProfissionalId == profissionalId);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusEspecialidade>(status, true, out var statusEnum))
        {
            query = query.Where(a => a.Status == statusEnum);
        }

        var total = await query.CountAsync();
        var agendas = await query
            .OrderByDescending(a => a.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new AgendasPaginadasDto
        {
            Dados = agendas.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<AgendaDto?> ObterAgendaPorIdAsync(Guid id)
    {
        var agenda = await _context.Agendas
            .Include(a => a.Profissional)
            .FirstOrDefaultAsync(a => a.Id == id);

        return agenda != null ? MapearParaDto(agenda) : null;
    }

    public async Task<AgendaDto?> ObterAgendaPorProfissionalAsync(Guid profissionalId)
    {
        var agenda = await _context.Agendas
            .Include(a => a.Profissional)
            .FirstOrDefaultAsync(a => a.ProfissionalId == profissionalId && a.Status == StatusEspecialidade.Ativa);

        return agenda != null ? MapearParaDto(agenda) : null;
    }

    public async Task<AgendaDto> CriarAgendaAsync(CriarAgendaDto dto)
    {
        var agendaExistente = await _context.Agendas
            .AnyAsync(a => a.ProfissionalId == dto.ProfissionalId && a.Status == StatusEspecialidade.Ativa);

        if (agendaExistente)
        {
            throw new InvalidOperationException("Profissional já possui uma agenda ativa.");
        }

        var agenda = new Agenda
        {
            ProfissionalId = dto.ProfissionalId,
            ConfiguracaoGlobalJson = JsonSerializer.Serialize(dto.ConfiguracaoGlobal),
            ConfiguracaoDiasJson = JsonSerializer.Serialize(dto.ConfiguracaoDias),
            DataInicioVigencia = DateTime.Parse(dto.DataInicioVigencia),
            DataFimVigencia = !string.IsNullOrEmpty(dto.DataFimVigencia) ? DateTime.Parse(dto.DataFimVigencia) : null,
            Status = StatusEspecialidade.Ativa,
            CriadoEm = DateTime.UtcNow
        };

        _context.Agendas.Add(agenda);
        await _context.SaveChangesAsync();

        return await ObterAgendaPorIdAsync(agenda.Id) ?? throw new InvalidOperationException("Erro ao criar agenda.");
    }

    public async Task<AgendaDto?> AtualizarAgendaAsync(Guid id, AtualizarAgendaDto dto)
    {
        var agenda = await _context.Agendas.FindAsync(id);
        if (agenda == null)
        {
            return null;
        }

        if (dto.ConfiguracaoGlobal != null)
        {
            agenda.ConfiguracaoGlobalJson = JsonSerializer.Serialize(dto.ConfiguracaoGlobal);
        }

        if (dto.ConfiguracaoDias != null)
        {
            agenda.ConfiguracaoDiasJson = JsonSerializer.Serialize(dto.ConfiguracaoDias);
        }

        if (!string.IsNullOrEmpty(dto.DataInicioVigencia))
        {
            agenda.DataInicioVigencia = DateTime.Parse(dto.DataInicioVigencia);
        }

        if (dto.DataFimVigencia != null)
        {
            agenda.DataFimVigencia = !string.IsNullOrEmpty(dto.DataFimVigencia) ? DateTime.Parse(dto.DataFimVigencia) : null;
        }

        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<StatusEspecialidade>(dto.Status, true, out var statusEnum))
        {
            agenda.Status = statusEnum;
        }

        agenda.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await ObterAgendaPorIdAsync(id);
    }

    public async Task<bool> DeletarAgendaAsync(Guid id)
    {
        var agenda = await _context.Agendas.FindAsync(id);
        if (agenda == null)
        {
            return false;
        }

        _context.Agendas.Remove(agenda);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AtivarAgendaAsync(Guid id)
    {
        var agenda = await _context.Agendas.FindAsync(id);
        if (agenda == null)
        {
            return false;
        }

        agenda.Status = StatusEspecialidade.Ativa;
        agenda.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesativarAgendaAsync(Guid id)
    {
        var agenda = await _context.Agendas.FindAsync(id);
        if (agenda == null)
        {
            return false;
        }

        agenda.Status = StatusEspecialidade.Inativa;
        agenda.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProfissionalDisponivelAsync(Guid profissionalId, DateTime data, string horario)
    {
        // Verificar se tem agenda ativa
        var agenda = await _context.Agendas
            .FirstOrDefaultAsync(a => a.ProfissionalId == profissionalId && a.Status == StatusEspecialidade.Ativa);

        if (agenda == null)
        {
            return false;
        }

        // Verificar se dia está dentro da vigência
        if (data < agenda.DataInicioVigencia || (agenda.DataFimVigencia.HasValue && data > agenda.DataFimVigencia))
        {
            return false;
        }

        // Verificar se já existe consulta neste horário
        var horarioTimeSpan = TimeSpan.Parse(horario);
        var consultaExistente = await _context.Consultas
            .AnyAsync(c => c.ProfissionalId == profissionalId &&
                          c.Data.Date == data.Date &&
                          c.Horario == horarioTimeSpan &&
                          c.Status != StatusConsulta.Cancelada);

        if (consultaExistente)
        {
            return false;
        }

        // Verificar bloqueios
        var bloqueioExistente = await _context.BloqueiosAgenda
            .AnyAsync(b => b.ProfissionalId == profissionalId &&
                          b.Status == StatusBloqueioAgenda.Aprovado &&
                          ((b.Data.HasValue && b.Data.Value.Date == data.Date) ||
                           (b.DataInicio.HasValue && b.DataFim.HasValue && 
                            data.Date >= b.DataInicio.Value.Date && data.Date <= b.DataFim.Value.Date)));

        return !bloqueioExistente;
    }

    public async Task<List<DateTime>> ObterDiasDisponiveisNoMesAsync(Guid profissionalId, int ano, int mes)
    {
        var diasDisponiveis = new List<DateTime>();

        var agenda = await _context.Agendas
            .FirstOrDefaultAsync(a => a.ProfissionalId == profissionalId && a.Status == StatusEspecialidade.Ativa);

        if (agenda == null || string.IsNullOrEmpty(agenda.ConfiguracaoDiasJson))
        {
            return diasDisponiveis;
        }

        try
        {
            var configuracaoDias = JsonSerializer.Deserialize<List<ConfiguracaoDiaJson>>(agenda.ConfiguracaoDiasJson);
            var diasAtivos = configuracaoDias?.Where(d => d.Ativo).Select(d => d.DiaSemana).ToList() ?? new List<int>();

            var primeiroDia = new DateTime(ano, mes, 1);
            var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);

            for (var data = primeiroDia; data <= ultimoDia; data = data.AddDays(1))
            {
                if (diasAtivos.Contains((int)data.DayOfWeek))
                {
                    // Verificar se está dentro da vigência
                    if (data >= agenda.DataInicioVigencia && (!agenda.DataFimVigencia.HasValue || data <= agenda.DataFimVigencia))
                    {
                        diasDisponiveis.Add(data);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dias disponíveis no mês");
        }

        return diasDisponiveis;
    }

    private AgendaDto MapearParaDto(Agenda agenda)
    {
        ConfiguracaoGlobalDto? configGlobal = null;
        List<ConfiguracaoDiaDto>? configDias = null;

        try
        {
            if (!string.IsNullOrEmpty(agenda.ConfiguracaoGlobalJson))
            {
                configGlobal = JsonSerializer.Deserialize<ConfiguracaoGlobalDto>(agenda.ConfiguracaoGlobalJson);
            }
            if (!string.IsNullOrEmpty(agenda.ConfiguracaoDiasJson))
            {
                configDias = JsonSerializer.Deserialize<List<ConfiguracaoDiaDto>>(agenda.ConfiguracaoDiasJson);
            }
        }
        catch { }

        return new AgendaDto
        {
            Id = agenda.Id,
            ProfissionalId = agenda.ProfissionalId,
            NomeProfissional = agenda.Profissional != null ? $"{agenda.Profissional.Nome} {agenda.Profissional.Sobrenome}".Trim() : null,
            ConfiguracaoGlobal = configGlobal,
            ConfiguracaoDias = configDias,
            DataInicioVigencia = agenda.DataInicioVigencia.ToString("yyyy-MM-dd"),
            DataFimVigencia = agenda.DataFimVigencia?.ToString("yyyy-MM-dd"),
            Status = agenda.Status?.ToString() ?? "Ativa",
            CriadoEm = agenda.CriadoEm,
            AtualizadoEm = agenda.AtualizadoEm
        };
    }

    private class ConfiguracaoDiaJson
    {
        public int DiaSemana { get; set; }
        public bool Ativo { get; set; }
    }
}
