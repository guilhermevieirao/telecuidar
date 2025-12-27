using Application.DTOs.BloqueiosAgenda;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de bloqueios de agenda
/// </summary>
public class BloqueioAgendaService : IBloqueioAgendaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BloqueioAgendaService> _logger;

    public BloqueioAgendaService(ApplicationDbContext context, ILogger<BloqueioAgendaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BloqueiosAgendaPaginadosDto> ObterBloqueiosAsync(int pagina, int tamanhoPagina, Guid? profissionalId, string? status)
    {
        var query = _context.BloqueiosAgenda
            .Include(b => b.Profissional)
            .Include(b => b.AprovadoPor)
            .AsQueryable();

        if (profissionalId.HasValue)
        {
            query = query.Where(b => b.ProfissionalId == profissionalId);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusBloqueioAgenda>(status, true, out var statusEnum))
        {
            query = query.Where(b => b.Status == statusEnum);
        }

        var total = await query.CountAsync();
        var bloqueios = await query
            .OrderByDescending(b => b.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new BloqueiosAgendaPaginadosDto
        {
            Dados = bloqueios.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<BloqueioAgendaDto?> ObterBloqueioPorIdAsync(Guid id)
    {
        var bloqueio = await _context.BloqueiosAgenda
            .Include(b => b.Profissional)
            .Include(b => b.AprovadoPor)
            .FirstOrDefaultAsync(b => b.Id == id);

        return bloqueio != null ? MapearParaDto(bloqueio) : null;
    }

    public async Task<List<BloqueioAgendaDto>> ObterBloqueiosPorProfissionalAsync(Guid profissionalId, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var query = _context.BloqueiosAgenda
            .Include(b => b.AprovadoPor)
            .Where(b => b.ProfissionalId == profissionalId && b.Status == StatusBloqueioAgenda.Aprovado);

        if (dataInicio.HasValue)
        {
            query = query.Where(b => 
                (b.Data.HasValue && b.Data.Value >= dataInicio) ||
                (b.DataFim.HasValue && b.DataFim.Value >= dataInicio));
        }

        if (dataFim.HasValue)
        {
            query = query.Where(b => 
                (b.Data.HasValue && b.Data.Value <= dataFim) ||
                (b.DataInicio.HasValue && b.DataInicio.Value <= dataFim));
        }

        var bloqueios = await query.OrderBy(b => b.Data ?? b.DataInicio).ToListAsync();
        return bloqueios.Select(MapearParaDto).ToList();
    }

    public async Task<BloqueioAgendaDto> CriarBloqueioAsync(Guid profissionalId, CriarBloqueioAgendaDto dto)
    {
        var bloqueio = new BloqueioAgenda
        {
            ProfissionalId = profissionalId,
            Tipo = Enum.Parse<TipoBloqueioAgenda>(dto.Tipo, true),
            Motivo = dto.Motivo,
            Status = StatusBloqueioAgenda.Pendente,
            CriadoEm = DateTime.UtcNow
        };

        if (dto.Tipo == "Unico" && !string.IsNullOrEmpty(dto.Data))
        {
            bloqueio.Data = DateTime.Parse(dto.Data);
        }
        else if (dto.Tipo == "Periodo")
        {
            if (!string.IsNullOrEmpty(dto.DataInicio))
                bloqueio.DataInicio = DateTime.Parse(dto.DataInicio);
            if (!string.IsNullOrEmpty(dto.DataFim))
                bloqueio.DataFim = DateTime.Parse(dto.DataFim);
        }

        _context.BloqueiosAgenda.Add(bloqueio);
        await _context.SaveChangesAsync();

        return await ObterBloqueioPorIdAsync(bloqueio.Id) ?? throw new InvalidOperationException("Erro ao criar bloqueio.");
    }

    public async Task<BloqueioAgendaDto?> AtualizarBloqueioAsync(Guid id, AtualizarBloqueioAgendaDto dto)
    {
        var bloqueio = await _context.BloqueiosAgenda.FindAsync(id);
        if (bloqueio == null)
        {
            return null;
        }

        if (bloqueio.Status != StatusBloqueioAgenda.Pendente)
        {
            throw new InvalidOperationException("Apenas bloqueios pendentes podem ser atualizados.");
        }

        if (!string.IsNullOrEmpty(dto.Tipo))
        {
            bloqueio.Tipo = Enum.Parse<TipoBloqueioAgenda>(dto.Tipo, true);
        }

        if (!string.IsNullOrEmpty(dto.Motivo))
        {
            bloqueio.Motivo = dto.Motivo;
        }

        if (dto.Data != null)
        {
            bloqueio.Data = !string.IsNullOrEmpty(dto.Data) ? DateTime.Parse(dto.Data) : null;
        }

        if (dto.DataInicio != null)
        {
            bloqueio.DataInicio = !string.IsNullOrEmpty(dto.DataInicio) ? DateTime.Parse(dto.DataInicio) : null;
        }

        if (dto.DataFim != null)
        {
            bloqueio.DataFim = !string.IsNullOrEmpty(dto.DataFim) ? DateTime.Parse(dto.DataFim) : null;
        }

        bloqueio.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await ObterBloqueioPorIdAsync(id);
    }

    public async Task<bool> DeletarBloqueioAsync(Guid id)
    {
        var bloqueio = await _context.BloqueiosAgenda.FindAsync(id);
        if (bloqueio == null)
        {
            return false;
        }

        _context.BloqueiosAgenda.Remove(bloqueio);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AprovarBloqueioAsync(Guid id, Guid aprovadorId)
    {
        var bloqueio = await _context.BloqueiosAgenda.FindAsync(id);
        if (bloqueio == null || bloqueio.Status != StatusBloqueioAgenda.Pendente)
        {
            return false;
        }

        bloqueio.Status = StatusBloqueioAgenda.Aprovado;
        bloqueio.AprovadoPorId = aprovadorId;
        bloqueio.AprovadoEm = DateTime.UtcNow;
        bloqueio.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejeitarBloqueioAsync(Guid id, Guid aprovadorId, string motivo)
    {
        var bloqueio = await _context.BloqueiosAgenda.FindAsync(id);
        if (bloqueio == null || bloqueio.Status != StatusBloqueioAgenda.Pendente)
        {
            return false;
        }

        bloqueio.Status = StatusBloqueioAgenda.Rejeitado;
        bloqueio.AprovadoPorId = aprovadorId;
        bloqueio.MotivoRejeicao = motivo;
        bloqueio.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static BloqueioAgendaDto MapearParaDto(BloqueioAgenda bloqueio)
    {
        return new BloqueioAgendaDto
        {
            Id = bloqueio.Id,
            ProfissionalId = bloqueio.ProfissionalId,
            NomeProfissional = bloqueio.Profissional != null ? $"{bloqueio.Profissional.Nome} {bloqueio.Profissional.Sobrenome}".Trim() : null,
            Tipo = bloqueio.Tipo.ToString(),
            Data = bloqueio.Data?.ToString("yyyy-MM-dd"),
            DataInicio = bloqueio.DataInicio?.ToString("yyyy-MM-dd"),
            DataFim = bloqueio.DataFim?.ToString("yyyy-MM-dd"),
            Motivo = bloqueio.Motivo,
            Status = bloqueio.Status.ToString(),
            AprovadoPorId = bloqueio.AprovadoPorId,
            NomeAprovador = bloqueio.AprovadoPor != null ? $"{bloqueio.AprovadoPor.Nome} {bloqueio.AprovadoPor.Sobrenome}".Trim() : null,
            AprovadoEm = bloqueio.AprovadoEm,
            MotivoRejeicao = bloqueio.MotivoRejeicao,
            CriadoEm = bloqueio.CriadoEm,
            AtualizadoEm = bloqueio.AtualizadoEm
        };
    }
}
