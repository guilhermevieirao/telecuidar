using Application.DTOs.LogsAuditoria;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de logs de auditoria
/// </summary>
public class LogAuditoriaService : ILogAuditoriaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LogAuditoriaService> _logger;

    public LogAuditoriaService(ApplicationDbContext context, ILogger<LogAuditoriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LogsAuditoriaPaginadosDto> ObterLogsAsync(FiltrosLogAuditoriaDto filtros)
    {
        var query = _context.LogsAuditoria
            .Include(l => l.Usuario)
            .AsQueryable();

        if (filtros.UsuarioId.HasValue)
        {
            query = query.Where(l => l.UsuarioId == filtros.UsuarioId);
        }

        if (!string.IsNullOrEmpty(filtros.Acao))
        {
            query = query.Where(l => l.Acao.Contains(filtros.Acao));
        }

        if (!string.IsNullOrEmpty(filtros.Entidade))
        {
            query = query.Where(l => l.Entidade == filtros.Entidade);
        }

        if (filtros.DataInicio.HasValue)
        {
            query = query.Where(l => l.CriadoEm >= filtros.DataInicio.Value);
        }

        if (filtros.DataFim.HasValue)
        {
            query = query.Where(l => l.CriadoEm <= filtros.DataFim.Value);
        }

        var total = await query.CountAsync();
        var logs = await query
            .OrderByDescending(l => l.CriadoEm)
            .Skip((filtros.Pagina - 1) * filtros.TamanhoPagina)
            .Take(filtros.TamanhoPagina)
            .ToListAsync();

        return new LogsAuditoriaPaginadosDto
        {
            Dados = logs.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };
    }

    public async Task<LogAuditoriaDto?> ObterLogPorIdAsync(Guid id)
    {
        var log = await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.Id == id);

        return log != null ? MapearParaDto(log) : null;
    }

    public async Task RegistrarLogAsync(CriarLogAuditoriaDto dto)
    {
        var log = new LogAuditoria
        {
            UsuarioId = dto.UsuarioId,
            Acao = dto.Acao,
            Entidade = dto.Entidade ?? string.Empty,
                EntidadeId = dto.EntidadeId?.ToString(),
        };

        _context.LogsAuditoria.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task RegistrarAsync(Guid? usuarioId, string acao, string? entidade = null, Guid? entidadeId = null, object? dadosAntigos = null, object? dadosNovos = null, string? enderecoIp = null, string? userAgent = null)
    {
        try
        {
            var log = new LogAuditoria
            {
                UsuarioId = usuarioId,
                Acao = acao,
                Entidade = entidade ?? string.Empty,
                EntidadeId = entidadeId?.ToString(),
                DadosAntigos = dadosAntigos != null ? JsonSerializer.Serialize(dadosAntigos) : null,
                DadosNovos = dadosNovos != null ? JsonSerializer.Serialize(dadosNovos) : null,
                EnderecoIp = enderecoIp,
                UserAgent = userAgent,
                CriadoEm = DateTime.UtcNow
            };

            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar log de auditoria");
        }
    }

    public async Task<bool> DeletarLogsAntigosAsync(int diasRetencao)
    {
        try
        {
            var dataLimite = DateTime.UtcNow.AddDays(-diasRetencao);
            var logsAntigos = await _context.LogsAuditoria
                .Where(l => l.CriadoEm < dataLimite)
                .ToListAsync();

            _context.LogsAuditoria.RemoveRange(logsAntigos);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Removidos {Count} logs de auditoria com mais de {Days} dias", logsAntigos.Count, diasRetencao);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar logs antigos");
            return false;
        }
    }

    private static LogAuditoriaDto MapearParaDto(LogAuditoria log)
    {
        return new LogAuditoriaDto
        {
            Id = log.Id,
            UsuarioId = log.UsuarioId,
            NomeUsuario = log.Usuario != null ? $"{log.Usuario.Nome} {log.Usuario.Sobrenome}".Trim() : null,
            Acao = log.Acao,
            Entidade = log.Entidade,
            EntidadeId = !string.IsNullOrEmpty(log.EntidadeId) && Guid.TryParse(log.EntidadeId, out var entidadeGuid) ? entidadeGuid : null,
            DadosAntigos = log.DadosAntigos,
            DadosNovos = log.DadosNovos,
            EnderecoIp = log.EnderecoIp,
            UserAgent = log.UserAgent,
            CriadoEm = log.CriadoEm
        };
    }
}
