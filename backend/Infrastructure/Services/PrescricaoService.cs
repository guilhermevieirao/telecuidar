using Application.DTOs.Prescricoes;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de prescrições médicas
/// </summary>
public class PrescricaoService : IPrescricaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PrescricaoService> _logger;

    public PrescricaoService(ApplicationDbContext context, ILogger<PrescricaoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PrescricoesPaginadasDto> ObterPrescricoesAsync(FiltrosPrescricaoDto filtros)
    {
        var query = _context.Prescricoes
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Profissional)
            .AsQueryable();

        if (filtros.ConsultaId.HasValue)
        {
            query = query.Where(p => p.ConsultaId == filtros.ConsultaId);
        }

        if (filtros.PacienteId.HasValue)
        {
            query = query.Where(p => p.Consulta.PacienteId == filtros.PacienteId);
        }

        if (filtros.ProfissionalId.HasValue)
        {
            query = query.Where(p => p.Consulta.ProfissionalId == filtros.ProfissionalId);
        }

        if (filtros.DataInicio.HasValue)
        {
            query = query.Where(p => p.CriadoEm >= filtros.DataInicio.Value);
        }

        if (filtros.DataFim.HasValue)
        {
            query = query.Where(p => p.CriadoEm <= filtros.DataFim.Value);
        }

        var total = await query.CountAsync();
        var prescricoes = await query
            .OrderByDescending(p => p.CriadoEm)
            .Skip((filtros.Pagina - 1) * filtros.TamanhoPagina)
            .Take(filtros.TamanhoPagina)
            .ToListAsync();

        return new PrescricoesPaginadasDto
        {
            Dados = prescricoes.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };
    }

    public async Task<PrescricaoDto?> ObterPrescricaoPorIdAsync(Guid id)
    {
        var prescricao = await _context.Prescricoes
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Profissional)
            .FirstOrDefaultAsync(p => p.Id == id);

        return prescricao != null ? MapearParaDto(prescricao) : null;
    }

    public async Task<List<PrescricaoDto>> ObterPrescricoesPorConsultaAsync(Guid consultaId)
    {
        var prescricoes = await _context.Prescricoes
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Profissional)
            .Where(p => p.ConsultaId == consultaId)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return prescricoes.Select(MapearParaDto).ToList();
    }

    public async Task<List<PrescricaoDto>> ObterPrescricoesPorPacienteAsync(Guid pacienteId)
    {
        var prescricoes = await _context.Prescricoes
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Profissional)
            .Where(p => p.Consulta.PacienteId == pacienteId)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return prescricoes.Select(MapearParaDto).ToList();
    }

    public async Task<PrescricaoDto> CriarPrescricaoAsync(CriarPrescricaoDto dto)
    {
        var consulta = await _context.Consultas.FindAsync(dto.ConsultaId);
        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        var prescricao = new Prescricao
        {
            ConsultaId = dto.ConsultaId,
            Tipo = dto.Tipo,
            ItensJson = JsonSerializer.Serialize(dto.Itens),
            Observacoes = dto.Observacoes,
            ValidadeEmDias = dto.ValidadeEmDias,
            AssinadoDigitalmente = false,
            CriadoEm = DateTime.UtcNow
        };

        _context.Prescricoes.Add(prescricao);
        await _context.SaveChangesAsync();

        return await ObterPrescricaoPorIdAsync(prescricao.Id) ?? throw new InvalidOperationException("Erro ao criar prescrição.");
    }

    public async Task<PrescricaoDto?> AtualizarPrescricaoAsync(Guid id, AtualizarPrescricaoDto dto)
    {
        var prescricao = await _context.Prescricoes.FindAsync(id);
        if (prescricao == null)
        {
            return null;
        }

        if (prescricao.AssinadoDigitalmente)
        {
            throw new InvalidOperationException("Prescrição já assinada não pode ser alterada.");
        }

        if (!string.IsNullOrEmpty(dto.Tipo))
        {
            prescricao.Tipo = dto.Tipo;
        }

        if (dto.Itens != null)
        {
            prescricao.ItensJson = JsonSerializer.Serialize(dto.Itens);
        }

        if (dto.Observacoes != null)
        {
            prescricao.Observacoes = dto.Observacoes;
        }

        if (dto.ValidadeEmDias.HasValue)
        {
            prescricao.ValidadeEmDias = dto.ValidadeEmDias.Value;
        }

        prescricao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await ObterPrescricaoPorIdAsync(id);
    }

    public async Task<bool> DeletarPrescricaoAsync(Guid id)
    {
        var prescricao = await _context.Prescricoes.FindAsync(id);
        if (prescricao == null)
        {
            return false;
        }

        if (prescricao.AssinadoDigitalmente)
        {
            throw new InvalidOperationException("Prescrição já assinada não pode ser deletada.");
        }

        _context.Prescricoes.Remove(prescricao);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssinarPrescricaoAsync(Guid id, Guid profissionalId, string certificadoId)
    {
        var prescricao = await _context.Prescricoes
            .Include(p => p.Consulta)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prescricao == null)
        {
            return false;
        }

        if (prescricao.Consulta.ProfissionalId != profissionalId)
        {
            throw new InvalidOperationException("Apenas o profissional da consulta pode assinar a prescrição.");
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

        prescricao.AssinadoDigitalmente = true;
        prescricao.CertificadoId = Guid.TryParse(certificadoId, out var certGuid) ? certGuid : null;
        prescricao.DataAssinatura = DateTime.UtcNow;
        prescricao.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]> GerarPdfPrescricaoAsync(Guid id)
    {
        var prescricao = await _context.Prescricoes
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
            .Include(p => p.Consulta)
                .ThenInclude(c => c.Profissional)
                    .ThenInclude(p => p.PerfilProfissional)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prescricao == null)
        {
            throw new InvalidOperationException("Prescrição não encontrada.");
        }

        // Gerar HTML da prescrição
        var html = GerarHtmlPrescricao(prescricao);

        // Por enquanto, retornar o HTML como bytes (em produção usaria biblioteca de PDF)
        return Encoding.UTF8.GetBytes(html);
    }

    private static string GerarHtmlPrescricao(Prescricao prescricao)
    {
        var itens = new List<ItemPrescricaoDto>();
        try
        {
            if (!string.IsNullOrEmpty(prescricao.ItensJson))
            {
                itens = JsonSerializer.Deserialize<List<ItemPrescricaoDto>>(prescricao.ItensJson) ?? new List<ItemPrescricaoDto>();
            }
        }
        catch { }

        var paciente = prescricao.Consulta.Paciente;
        var profissional = prescricao.Consulta.Profissional;

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"pt-BR\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<title>Prescrição Médica</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
        html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
        html.AppendLine(".info { margin-bottom: 20px; }");
        html.AppendLine(".item { margin: 10px 0; padding: 10px; border-bottom: 1px solid #ccc; }");
        html.AppendLine(".footer { margin-top: 40px; text-align: center; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        
        html.AppendLine("<div class=\"header\">");
        html.AppendLine("<h1>Prescrição Médica</h1>");
        html.AppendLine($"<p>Data: {prescricao.CriadoEm:dd/MM/yyyy}</p>");
        html.AppendLine("</div>");

        html.AppendLine("<div class=\"info\">");
        html.AppendLine($"<p><strong>Paciente:</strong> {paciente?.Nome} {paciente?.Sobrenome}</p>");
        html.AppendLine($"<p><strong>Profissional:</strong> {profissional?.Nome} {profissional?.Sobrenome}</p>");
        if (profissional?.PerfilProfissional != null)
        {
            html.AppendLine($"<p><strong>Registro:</strong> {profissional.PerfilProfissional.NumeroRegistro} - {profissional.PerfilProfissional.TipoRegistro}</p>");
        }
        html.AppendLine("</div>");

        html.AppendLine("<div class=\"items\">");
        html.AppendLine("<h2>Medicamentos/Itens:</h2>");
        foreach (var item in itens)
        {
            html.AppendLine("<div class=\"item\">");
            html.AppendLine($"<p><strong>{item.Nome}</strong></p>");
            html.AppendLine($"<p>Dosagem: {item.Dosagem}</p>");
            html.AppendLine($"<p>Posologia: {item.Posologia}</p>");
            if (!string.IsNullOrEmpty(item.Observacao))
            {
                html.AppendLine($"<p>Obs: {item.Observacao}</p>");
            }
            html.AppendLine("</div>");
        }
        html.AppendLine("</div>");

        if (!string.IsNullOrEmpty(prescricao.Observacoes))
        {
            html.AppendLine($"<div class=\"observations\"><p><strong>Observações:</strong> {prescricao.Observacoes}</p></div>");
        }

        html.AppendLine("<div class=\"footer\">");
        if (prescricao.AssinadoDigitalmente)
        {
            html.AppendLine($"<p>Assinado digitalmente em: {prescricao.DataAssinatura:dd/MM/yyyy HH:mm}</p>");
        }
        html.AppendLine($"<p>Validade: {prescricao.ValidadeEmDias} dias</p>");
        html.AppendLine("</div>");

        html.AppendLine("</body></html>");

        return html.ToString();
    }

    private static PrescricaoDto MapearParaDto(Prescricao prescricao)
    {
        List<ItemPrescricaoDto>? itens = null;
        try
        {
            if (!string.IsNullOrEmpty(prescricao.ItensJson))
            {
                itens = JsonSerializer.Deserialize<List<ItemPrescricaoDto>>(prescricao.ItensJson);
            }
        }
        catch { }

        return new PrescricaoDto
        {
            Id = prescricao.Id,
            ConsultaId = prescricao.ConsultaId,
            NomePaciente = prescricao.Consulta?.Paciente != null 
                ? $"{prescricao.Consulta.Paciente.Nome} {prescricao.Consulta.Paciente.Sobrenome}".Trim() 
                : null,
            NomeProfissional = prescricao.Consulta?.Profissional != null 
                ? $"{prescricao.Consulta.Profissional.Nome} {prescricao.Consulta.Profissional.Sobrenome}".Trim() 
                : null,
            Tipo = prescricao.Tipo,
            Itens = itens,
            Observacoes = prescricao.Observacoes,
            ValidadeEmDias = prescricao.ValidadeEmDias ?? 0,
            AssinadoDigitalmente = prescricao.AssinadoDigitalmente,
            CertificadoId = prescricao.CertificadoId?.ToString(),
            DataAssinatura = prescricao.DataAssinatura,
            CriadoEm = prescricao.CriadoEm,
            AtualizadoEm = prescricao.AtualizadoEm
        };
    }
}
