using Application.DTOs.Notificacoes;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de notificações
/// </summary>
public class NotificacaoService : INotificacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificacaoService> _logger;

    public NotificacaoService(ApplicationDbContext context, ILogger<NotificacaoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NotificacoesPaginadasDto> ObterNotificacoesAsync(Guid usuarioId, int pagina, int tamanhoPagina, bool? apenasNaoLidas = null)
    {
        var query = _context.Notificacoes.Where(n => n.UsuarioId == usuarioId);

        if (apenasNaoLidas == true)
        {
            query = query.Where(n => !n.Lida);
        }

        var total = await query.CountAsync();
        var naoLidas = await _context.Notificacoes.CountAsync(n => n.UsuarioId == usuarioId && !n.Lida);

        var notificacoes = await query
            .OrderByDescending(n => n.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new NotificacoesPaginadasDto
        {
            Dados = notificacoes.Select(MapearParaDto).ToList(),
            Total = total,
            NaoLidas = naoLidas,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<NotificacaoDto?> ObterNotificacaoPorIdAsync(Guid id)
    {
        var notificacao = await _context.Notificacoes.FindAsync(id);
        return notificacao != null ? MapearParaDto(notificacao) : null;
    }

    public async Task<int> ContarNaoLidasAsync(Guid usuarioId)
    {
        return await _context.Notificacoes.CountAsync(n => n.UsuarioId == usuarioId && !n.Lida);
    }

    public async Task<NotificacaoDto> CriarNotificacaoAsync(CriarNotificacaoDto dto)
    {
        var notificacao = new Notificacao
        {
            UsuarioId = dto.UsuarioId,
            Titulo = dto.Titulo,
            Mensagem = dto.Mensagem,
            Tipo = dto.Tipo ?? "info",
            Link = dto.Link,
            Lida = false,
            CriadoEm = DateTime.UtcNow
        };

        _context.Notificacoes.Add(notificacao);
        await _context.SaveChangesAsync();

        return MapearParaDto(notificacao);
    }

    public async Task<bool> MarcarComoLidaAsync(Guid id)
    {
        var notificacao = await _context.Notificacoes.FindAsync(id);
        if (notificacao == null)
        {
            return false;
        }

        notificacao.Lida = true;
        notificacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarcarTodasComoLidasAsync(Guid usuarioId)
    {
        var notificacoes = await _context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId && !n.Lida)
            .ToListAsync();

        foreach (var notificacao in notificacoes)
        {
            notificacao.Lida = true;
            notificacao.AtualizadoEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarNotificacaoAsync(Guid id)
    {
        var notificacao = await _context.Notificacoes.FindAsync(id);
        if (notificacao == null)
        {
            return false;
        }

        _context.Notificacoes.Remove(notificacao);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarTodasAsync(Guid usuarioId)
    {
        var notificacoes = await _context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId)
            .ToListAsync();

        _context.Notificacoes.RemoveRange(notificacoes);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task EnviarNotificacaoNovaConsultaAsync(Guid pacienteId, Guid profissionalId, DateTime dataConsulta, string horario)
    {
        var paciente = await _context.Usuarios.FindAsync(pacienteId);
        var profissional = await _context.Usuarios.FindAsync(profissionalId);

        if (paciente != null)
        {
            await CriarNotificacaoAsync(new CriarNotificacaoDto
            {
                UsuarioId = pacienteId,
                Titulo = "Nova Consulta Agendada",
                Mensagem = $"Sua consulta com {profissional?.Nome} foi agendada para {dataConsulta:dd/MM/yyyy} às {horario}.",
                Tipo = "success",
                Link = "/consultas"
            });
        }

        if (profissional != null)
        {
            await CriarNotificacaoAsync(new CriarNotificacaoDto
            {
                UsuarioId = profissionalId,
                Titulo = "Nova Consulta Agendada",
                Mensagem = $"Consulta com {paciente?.Nome} agendada para {dataConsulta:dd/MM/yyyy} às {horario}.",
                Tipo = "info",
                Link = "/agenda"
            });
        }
    }

    public async Task EnviarNotificacaoConsultaCanceladaAsync(Guid pacienteId, Guid profissionalId, DateTime dataConsulta, string horario)
    {
        var paciente = await _context.Usuarios.FindAsync(pacienteId);
        var profissional = await _context.Usuarios.FindAsync(profissionalId);

        if (paciente != null)
        {
            await CriarNotificacaoAsync(new CriarNotificacaoDto
            {
                UsuarioId = pacienteId,
                Titulo = "Consulta Cancelada",
                Mensagem = $"Sua consulta de {dataConsulta:dd/MM/yyyy} às {horario} foi cancelada.",
                Tipo = "warning"
            });
        }

        if (profissional != null)
        {
            await CriarNotificacaoAsync(new CriarNotificacaoDto
            {
                UsuarioId = profissionalId,
                Titulo = "Consulta Cancelada",
                Mensagem = $"Consulta com {paciente?.Nome} de {dataConsulta:dd/MM/yyyy} às {horario} foi cancelada.",
                Tipo = "warning"
            });
        }
    }

    public async Task EnviarNotificacaoLembreteConsultaAsync(Guid usuarioId, DateTime dataConsulta, string horario)
    {
        await CriarNotificacaoAsync(new CriarNotificacaoDto
        {
            UsuarioId = usuarioId,
            Titulo = "Lembrete de Consulta",
            Mensagem = $"Sua consulta está marcada para {dataConsulta:dd/MM/yyyy} às {horario}.",
            Tipo = "info",
            Link = "/consultas"
        });
    }

    private static NotificacaoDto MapearParaDto(Notificacao notificacao)
    {
        return new NotificacaoDto
        {
            Id = notificacao.Id,
            UsuarioId = notificacao.UsuarioId,
            Titulo = notificacao.Titulo,
            Mensagem = notificacao.Mensagem,
            Tipo = notificacao.Tipo,
            Lida = notificacao.Lida,
            Link = notificacao.Link,
            CriadoEm = notificacao.CriadoEm
        };
    }
}
