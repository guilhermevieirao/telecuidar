using Application.DTOs.Atestados;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de atestados médicos
/// </summary>
public class AtestadoMedicoService : IAtestadoMedicoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AtestadoMedicoService> _logger;

    public AtestadoMedicoService(ApplicationDbContext context, ILogger<AtestadoMedicoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AtestadosPaginadosDto> ObterAtestadosAsync(FiltrosAtestadoDto filtros)
    {
        var query = _context.AtestadosMedicos
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Profissional)
            .AsQueryable();

        if (filtros.ConsultaId.HasValue)
        {
            query = query.Where(a => a.ConsultaId == filtros.ConsultaId);
        }

        if (filtros.PacienteId.HasValue)
        {
            query = query.Where(a => a.Consulta.PacienteId == filtros.PacienteId);
        }

        if (filtros.ProfissionalId.HasValue)
        {
            query = query.Where(a => a.Consulta.ProfissionalId == filtros.ProfissionalId);
        }

        if (!string.IsNullOrEmpty(filtros.Tipo) && Enum.TryParse<TipoAtestado>(filtros.Tipo, true, out var tipoEnum))
        {
            query = query.Where(a => a.Tipo == tipoEnum);
        }

        if (filtros.DataInicio.HasValue)
        {
            query = query.Where(a => a.CriadoEm >= filtros.DataInicio.Value);
        }

        if (filtros.DataFim.HasValue)
        {
            query = query.Where(a => a.CriadoEm <= filtros.DataFim.Value);
        }

        var total = await query.CountAsync();
        var atestados = await query
            .OrderByDescending(a => a.CriadoEm)
            .Skip((filtros.Pagina - 1) * filtros.TamanhoPagina)
            .Take(filtros.TamanhoPagina)
            .ToListAsync();

        return new AtestadosPaginadosDto
        {
            Dados = atestados.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };
    }

    public async Task<AtestadoDto?> ObterAtestadoPorIdAsync(Guid id)
    {
        var atestado = await _context.AtestadosMedicos
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Profissional)
            .FirstOrDefaultAsync(a => a.Id == id);

        return atestado != null ? MapearParaDto(atestado) : null;
    }

    public async Task<List<AtestadoDto>> ObterAtestadosPorConsultaAsync(Guid consultaId)
    {
        var atestados = await _context.AtestadosMedicos
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Profissional)
            .Where(a => a.ConsultaId == consultaId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

        return atestados.Select(MapearParaDto).ToList();
    }

    public async Task<List<AtestadoDto>> ObterAtestadosPorPacienteAsync(Guid pacienteId)
    {
        var atestados = await _context.AtestadosMedicos
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Profissional)
            .Where(a => a.Consulta.PacienteId == pacienteId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

        return atestados.Select(MapearParaDto).ToList();
    }

    public async Task<AtestadoDto> CriarAtestadoAsync(CriarAtestadoDto dto)
    {
        var consulta = await _context.Consultas.FindAsync(dto.ConsultaId);
        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        var atestado = new AtestadoMedico
        {
            ConsultaId = dto.ConsultaId,
            Tipo = Enum.Parse<TipoAtestado>(dto.Tipo, true),
            DataInicio = DateTime.Parse(dto.DataInicio),
            DataFim = DateTime.Parse(dto.DataFim),
            DiasTotais = dto.DiasTotais,
            Conteudo = dto.Conteudo,
            Cid = dto.Cid ?? string.Empty,
            AssinadoDigitalmente = false,
            CriadoEm = DateTime.UtcNow
        };

        _context.AtestadosMedicos.Add(atestado);
        await _context.SaveChangesAsync();

        return await ObterAtestadoPorIdAsync(atestado.Id) ?? throw new InvalidOperationException("Erro ao criar atestado.");
    }

    public async Task<AtestadoDto?> AtualizarAtestadoAsync(Guid id, AtualizarAtestadoDto dto)
    {
        var atestado = await _context.AtestadosMedicos.FindAsync(id);
        if (atestado == null)
        {
            return null;
        }

        if (atestado.AssinadoDigitalmente)
        {
            throw new InvalidOperationException("Atestado já assinado não pode ser alterado.");
        }

        if (!string.IsNullOrEmpty(dto.Tipo) && Enum.TryParse<TipoAtestado>(dto.Tipo, true, out var tipoEnum))
        {
            atestado.Tipo = tipoEnum;
        }

        if (!string.IsNullOrEmpty(dto.DataInicio))
        {
            atestado.DataInicio = DateTime.Parse(dto.DataInicio);
        }

        if (!string.IsNullOrEmpty(dto.DataFim))
        {
            atestado.DataFim = DateTime.Parse(dto.DataFim);
        }

        if (dto.DiasTotais.HasValue)
        {
            atestado.DiasTotais = dto.DiasTotais.Value;
        }

        if (dto.Conteudo != null)
        {
            atestado.Conteudo = dto.Conteudo;
        }

        if (dto.Cid != null)
        {
            atestado.Cid = dto.Cid;
        }

        atestado.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await ObterAtestadoPorIdAsync(id);
    }

    public async Task<bool> DeletarAtestadoAsync(Guid id)
    {
        var atestado = await _context.AtestadosMedicos.FindAsync(id);
        if (atestado == null)
        {
            return false;
        }

        if (atestado.AssinadoDigitalmente)
        {
            throw new InvalidOperationException("Atestado já assinado não pode ser deletado.");
        }

        _context.AtestadosMedicos.Remove(atestado);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssinarAtestadoAsync(Guid id, Guid profissionalId, string certificadoId)
    {
        var atestado = await _context.AtestadosMedicos
            .Include(a => a.Consulta)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (atestado == null)
        {
            return false;
        }

        if (atestado.Consulta.ProfissionalId != profissionalId)
        {
            throw new InvalidOperationException("Apenas o profissional da consulta pode assinar o atestado.");
        }

        // Verificar se o certificado existe e está válido
        var certificado = await _context.CertificadosSalvos
            .FirstOrDefaultAsync(c => c.Id.ToString() == certificadoId && 
                                      c.UsuarioId == profissionalId &&
                                      c.DataValidade > DateTime.UtcNow);

        if (certificado == null)
        {
            throw new InvalidOperationException("Certificado digital não encontrado ou expirado.");
        }

        atestado.AssinadoDigitalmente = true;
        atestado.CertificadoId = Guid.TryParse(certificadoId, out var certGuid) ? certGuid : null;
        atestado.DataAssinatura = DateTime.UtcNow;
        atestado.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]> GerarPdfAtestadoAsync(Guid id)
    {
        var atestado = await _context.AtestadosMedicos
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(a => a.Consulta)
                .ThenInclude(c => c.Profissional)
                    .ThenInclude(p => p.PerfilProfissional)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (atestado == null)
        {
            throw new InvalidOperationException("Atestado não encontrado.");
        }

        // Gerar HTML do atestado
        var html = GerarHtmlAtestado(atestado);

        // Por enquanto, retornar o HTML como bytes (em produção usaria biblioteca de PDF)
        return Encoding.UTF8.GetBytes(html);
    }

    private static string GerarHtmlAtestado(AtestadoMedico atestado)
    {
        var paciente = atestado.Consulta.Paciente;
        var profissional = atestado.Consulta.Profissional;

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"pt-BR\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<title>Atestado Médico</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
        html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
        html.AppendLine(".content { margin: 30px 0; line-height: 1.8; }");
        html.AppendLine(".footer { margin-top: 60px; text-align: center; }");
        html.AppendLine(".signature { margin-top: 40px; text-align: center; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("<div class=\"header\">");
        html.AppendLine("<h1>ATESTADO MÉDICO</h1>");
        html.AppendLine($"<p>Data: {atestado.CriadoEm:dd/MM/yyyy}</p>");
        html.AppendLine("</div>");

        html.AppendLine("<div class=\"content\">");
        html.AppendLine($"<p>Atesto para os devidos fins que o(a) paciente <strong>{paciente?.Nome} {paciente?.Sobrenome}</strong>, ");
        html.AppendLine($"foi atendido(a) em consulta médica em {atestado.Consulta.Data:dd/MM/yyyy}.</p>");
        
        if (!string.IsNullOrEmpty(atestado.Conteudo))
        {
            html.AppendLine($"<p>{atestado.Conteudo}</p>");
        }

        html.AppendLine($"<p>Período de afastamento: <strong>{atestado.DataInicio:dd/MM/yyyy}</strong> a <strong>{atestado.DataFim:dd/MM/yyyy}</strong></p>");
        html.AppendLine($"<p>Total de dias: <strong>{atestado.DiasTotais} dia(s)</strong></p>");

        if (!string.IsNullOrEmpty(atestado.Cid))
        {
            html.AppendLine($"<p>CID-10: <strong>{atestado.Cid}</strong></p>");
        }
        html.AppendLine("</div>");

        html.AppendLine("<div class=\"signature\">");
        html.AppendLine("<p>_______________________________________</p>");
        html.AppendLine($"<p><strong>{profissional?.Nome} {profissional?.Sobrenome}</strong></p>");
        if (profissional?.PerfilProfissional != null)
        {
            html.AppendLine($"<p>{profissional.PerfilProfissional.TipoRegistro}: {profissional.PerfilProfissional.NumeroRegistro}</p>");
        }
        html.AppendLine("</div>");

        html.AppendLine("<div class=\"footer\">");
        if (atestado.AssinadoDigitalmente)
        {
            html.AppendLine($"<p><em>Documento assinado digitalmente em {atestado.DataAssinatura:dd/MM/yyyy HH:mm}</em></p>");
        }
        html.AppendLine("</div>");

        html.AppendLine("</body></html>");

        return html.ToString();
    }

    private static AtestadoDto MapearParaDto(AtestadoMedico atestado)
    {
        return new AtestadoDto
        {
            Id = atestado.Id,
            ConsultaId = atestado.ConsultaId,
            NomePaciente = atestado.Consulta?.Paciente != null
                ? $"{atestado.Consulta.Paciente.Nome} {atestado.Consulta.Paciente.Sobrenome}".Trim()
                : null,
            NomeProfissional = atestado.Consulta?.Profissional != null
                ? $"{atestado.Consulta.Profissional.Nome} {atestado.Consulta.Profissional.Sobrenome}".Trim()
                : null,
            Tipo = atestado.Tipo.ToString(),
            DataInicio = atestado.DataInicio?.ToString("yyyy-MM-dd") ?? string.Empty,
            DataFim = atestado.DataFim?.ToString("yyyy-MM-dd") ?? string.Empty,
            DiasTotais = atestado.DiasTotais,
            Conteudo = atestado.Conteudo,
            Cid = atestado.Cid,
            AssinadoDigitalmente = atestado.AssinadoDigitalmente,
            CertificadoId = atestado.CertificadoId?.ToString(),
            DataAssinatura = atestado.DataAssinatura,
            CriadoEm = atestado.CriadoEm,
            AtualizadoEm = atestado.AtualizadoEm
        };
    }
}
