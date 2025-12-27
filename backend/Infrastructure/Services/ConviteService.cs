using Application.DTOs.Convites;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de convites
/// </summary>
public class ConviteService : IConviteService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ConviteService> _logger;

    public ConviteService(
        ApplicationDbContext context,
        IEmailService emailService,
        ILogger<ConviteService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ConvitesPaginadosDto> ObterConvitesAsync(int pagina, int tamanhoPagina, string? status)
    {
        var query = _context.Convites
            .Include(c => c.CriadoPor)
            .Include(c => c.Especialidade)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusConvite>(status, true, out var statusEnum))
        {
            query = query.Where(c => c.Status == statusEnum);
        }

        var total = await query.CountAsync();
        var convites = await query
            .OrderByDescending(c => c.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new ConvitesPaginadosDto
        {
            Dados = convites.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<ConviteDto?> ObterConvitePorIdAsync(Guid id)
    {
        var convite = await _context.Convites
            .Include(c => c.CriadoPor)
            .Include(c => c.Especialidade)
            .FirstOrDefaultAsync(c => c.Id == id);

        return convite != null ? MapearParaDto(convite) : null;
    }

    public async Task<ConviteDto> CriarConviteAsync(CriarConviteDto dto, Guid criadorId)
    {
        var diasValidade = dto.DiasValidade ?? 7;

        var convite = new Convite
        {
            Email = dto.Email,
            TipoUsuario = Enum.Parse<TipoUsuario>(dto.TipoUsuario, true),
            EspecialidadeId = dto.EspecialidadeId,
            Token = Guid.NewGuid().ToString("N"),
            Status = StatusConvite.Pendente,
            ExpiraEm = DateTime.UtcNow.AddDays(diasValidade),
            CriadoPorId = criadorId,
            CriadoEm = DateTime.UtcNow
        };

        _context.Convites.Add(convite);
        await _context.SaveChangesAsync();

        // Enviar e-mail de convite se tiver email
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var criador = await _context.Usuarios.FindAsync(criadorId);
            var nomeCriador = criador != null ? $"{criador.Nome} {criador.Sobrenome}".Trim() : null;

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.EnviarEmailConviteAsync(dto.Email, convite.Token, dto.TipoUsuario, nomeCriador);
                    _logger.LogInformation("E-mail de convite enviado para {Email}", dto.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar e-mail de convite para {Email}", dto.Email);
                }
            });
        }

        return await ObterConvitePorIdAsync(convite.Id) ?? throw new InvalidOperationException("Erro ao criar convite.");
    }

    public async Task<bool> RevogarConviteAsync(Guid id)
    {
        var convite = await _context.Convites.FindAsync(id);
        if (convite == null)
        {
            return false;
        }

        convite.Status = StatusConvite.Revogado;
        convite.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ValidarConviteResponseDto> ValidarTokenAsync(string token)
    {
        var convite = await _context.Convites
            .Include(c => c.Especialidade)
            .FirstOrDefaultAsync(c => c.Token == token);

        if (convite == null)
        {
            return new ValidarConviteResponseDto
            {
                Valido = false,
                Mensagem = "Convite não encontrado."
            };
        }

        if (convite.Status != StatusConvite.Pendente)
        {
            return new ValidarConviteResponseDto
            {
                Valido = false,
                Mensagem = $"Convite já foi {convite.Status.ToString().ToLower()}."
            };
        }

        if (convite.ExpiraEm < DateTime.UtcNow)
        {
            return new ValidarConviteResponseDto
            {
                Valido = false,
                Mensagem = "Convite expirado."
            };
        }

        return new ValidarConviteResponseDto
        {
            Valido = true,
            Mensagem = "Convite válido.",
            Email = convite.Email,
            TipoUsuario = convite.TipoUsuario.ToString(),
            EspecialidadeId = convite.EspecialidadeId
        };
    }

    public async Task<bool> AceitarConviteAsync(string token, Guid usuarioId)
    {
        var convite = await _context.Convites.FirstOrDefaultAsync(c => c.Token == token);
        if (convite == null || convite.Status != StatusConvite.Pendente || convite.ExpiraEm < DateTime.UtcNow)
        {
            return false;
        }

        convite.Status = StatusConvite.Aceito;
        convite.AceitoEm = DateTime.UtcNow;
        convite.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ConviteDto?> ObterConvitePorTokenAsync(string token)
    {
        var convite = await _context.Convites
            .Include(c => c.CriadoPor)
            .Include(c => c.Especialidade)
            .FirstOrDefaultAsync(c => c.Token == token);

        return convite != null ? MapearParaDto(convite) : null;
    }

    private static ConviteDto MapearParaDto(Convite convite)
    {
        return new ConviteDto
        {
            Id = convite.Id,
            Email = convite.Email,
            TipoUsuario = convite.TipoUsuario.ToString(),
            EspecialidadeId = convite.EspecialidadeId,
            NomeEspecialidade = convite.Especialidade?.Nome,
            Token = convite.Token,
            Status = convite.Status.ToString(),
            ExpiraEm = convite.ExpiraEm,
            CriadoPorId = convite.CriadoPorId,
            NomeCriador = convite.CriadoPor != null ? $"{convite.CriadoPor.Nome} {convite.CriadoPor.Sobrenome}".Trim() : null,
            CriadoEm = convite.CriadoEm,
            AceitoEm = convite.AceitoEm
        };
    }
}
