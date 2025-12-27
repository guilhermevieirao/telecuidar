using Application.DTOs.HistoricosClinico;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de histórico clínico do paciente
/// </summary>
public class HistoricoClinicoService : IHistoricoClinicoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HistoricoClinicoService> _logger;

    public HistoricoClinicoService(ApplicationDbContext context, ILogger<HistoricoClinicoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HistoricoClinicoDto?> ObterHistoricoPorPacienteAsync(Guid pacienteId)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        return historico != null ? MapearParaDto(historico) : null;
    }

    public async Task<HistoricoClinicoDto> CriarOuAtualizarHistoricoAsync(Guid pacienteId, AtualizarHistoricoClinicoDto dto)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        if (historico == null)
        {
            historico = new HistoricoClinico
            {
                PacienteId = pacienteId,
                CriadoEm = DateTime.UtcNow
            };
            _context.HistoricosClinicos.Add(historico);
        }

        // Atualizar campos
        if (dto.Alergias != null)
            historico.AlergiasJson = JsonSerializer.Serialize(dto.Alergias);

        if (dto.MedicamentosEmUso != null)
            historico.MedicamentosEmUsoJson = JsonSerializer.Serialize(dto.MedicamentosEmUso);

        if (dto.DoencasCronicas != null)
            historico.DoencasCronicasJson = JsonSerializer.Serialize(dto.DoencasCronicas);

        if (dto.CirurgiasAnteriores != null)
            historico.CirurgiasAnterioresJson = JsonSerializer.Serialize(dto.CirurgiasAnteriores);

        if (dto.HistoricoFamiliar != null)
            historico.HistoricoFamiliarJson = JsonSerializer.Serialize(dto.HistoricoFamiliar);

        if (dto.Vacinacoes != null)
            historico.VacinacoesJson = JsonSerializer.Serialize(dto.Vacinacoes);

        if (dto.HabitosSociais != null)
            historico.HabitosSociaisJson = JsonSerializer.Serialize(dto.HabitosSociais);

        if (dto.TipoSanguineo != null)
            historico.TipoSanguineo = dto.TipoSanguineo;

        if (dto.Observacoes != null)
            historico.Observacoes = dto.Observacoes;

        historico.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapearParaDto(historico);
    }

    public async Task<HistoricoConsultasDto> ObterHistoricoConsultasAsync(Guid pacienteId, int pagina = 1, int tamanhoPagina = 10)
    {
        var query = _context.Consultas
            .Include(c => c.Profissional)
            .Include(c => c.Especialidade)
            .Include(c => c.RegistroSoap)
            .Include(c => c.Prescricoes)
            .Include(c => c.Atestados)
            .Where(c => c.PacienteId == pacienteId && c.Status == StatusConsulta.Realizada);

        var total = await query.CountAsync();
        var consultas = await query
            .OrderByDescending(c => c.Data)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new HistoricoConsultasDto
        {
            Consultas = consultas.Select(c => new ConsultaHistoricoDto
            {
                Id = c.Id,
                Data = c.Data,
                Horario = c.Horario.ToString(@"hh\:mm"),
                NomeProfissional = c.Profissional != null ? $"{c.Profissional.Nome} {c.Profissional.Sobrenome}".Trim() : null,
                Especialidade = c.Especialidade?.Nome,
                Subjetivo = c.RegistroSoap?.Subjetivo,
                Objetivo = c.RegistroSoap?.Objetivo,
                Avaliacao = c.RegistroSoap?.Avaliacao,
                Plano = c.RegistroSoap?.Plano,
                TemPrescricao = c.Prescricoes.Any(),
                TemAtestado = c.Atestados.Any()
            }).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<List<MedicamentoEmUsoDto>> ObterMedicamentosEmUsoAsync(Guid pacienteId)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        if (historico == null || string.IsNullOrEmpty(historico.MedicamentosEmUsoJson))
        {
            return new List<MedicamentoEmUsoDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<MedicamentoEmUsoDto>>(historico.MedicamentosEmUsoJson) 
                   ?? new List<MedicamentoEmUsoDto>();
        }
        catch
        {
            return new List<MedicamentoEmUsoDto>();
        }
    }

    public async Task<List<AlergiaDto>> ObterAlergiasAsync(Guid pacienteId)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        if (historico == null || string.IsNullOrEmpty(historico.AlergiasJson))
        {
            return new List<AlergiaDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<AlergiaDto>>(historico.AlergiasJson) 
                   ?? new List<AlergiaDto>();
        }
        catch
        {
            return new List<AlergiaDto>();
        }
    }

    public async Task<bool> AdicionarAlergiaAsync(Guid pacienteId, AlergiaDto alergia)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        if (historico == null)
        {
            historico = new HistoricoClinico
            {
                PacienteId = pacienteId,
                CriadoEm = DateTime.UtcNow
            };
            _context.HistoricosClinicos.Add(historico);
        }

        var alergias = new List<AlergiaDto>();
        if (!string.IsNullOrEmpty(historico.AlergiasJson))
        {
            try { alergias = JsonSerializer.Deserialize<List<AlergiaDto>>(historico.AlergiasJson) ?? new List<AlergiaDto>(); }
            catch { }
        }

        alergias.Add(alergia);
        historico.AlergiasJson = JsonSerializer.Serialize(alergias);
        historico.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdicionarMedicamentoEmUsoAsync(Guid pacienteId, MedicamentoEmUsoDto medicamento)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        if (historico == null)
        {
            historico = new HistoricoClinico
            {
                PacienteId = pacienteId,
                CriadoEm = DateTime.UtcNow
            };
            _context.HistoricosClinicos.Add(historico);
        }

        var medicamentos = new List<MedicamentoEmUsoDto>();
        if (!string.IsNullOrEmpty(historico.MedicamentosEmUsoJson))
        {
            try { medicamentos = JsonSerializer.Deserialize<List<MedicamentoEmUsoDto>>(historico.MedicamentosEmUsoJson) ?? new List<MedicamentoEmUsoDto>(); }
            catch { }
        }

        medicamentos.Add(medicamento);
        historico.MedicamentosEmUsoJson = JsonSerializer.Serialize(medicamentos);
        historico.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ResumoClinicoDto> ObterResumoClinicoAsync(Guid pacienteId)
    {
        var historico = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.PacienteId == pacienteId);

        var ultimasConsultas = await _context.Consultas
            .Include(c => c.Especialidade)
            .Where(c => c.PacienteId == pacienteId && c.Status == StatusConsulta.Realizada)
            .OrderByDescending(c => c.Data)
            .Take(5)
            .Select(c => new { c.Data, Especialidade = c.Especialidade != null ? c.Especialidade.Nome : null })
            .ToListAsync();

        var totalConsultas = await _context.Consultas
            .CountAsync(c => c.PacienteId == pacienteId && c.Status == StatusConsulta.Realizada);

        var alergias = new List<AlergiaDto>();
        var medicamentos = new List<MedicamentoEmUsoDto>();
        var doencasCronicas = new List<string>();

        if (historico != null)
        {
            if (!string.IsNullOrEmpty(historico.AlergiasJson))
            {
                try { alergias = JsonSerializer.Deserialize<List<AlergiaDto>>(historico.AlergiasJson) ?? new List<AlergiaDto>(); }
                catch { }
            }
            if (!string.IsNullOrEmpty(historico.MedicamentosEmUsoJson))
            {
                try { medicamentos = JsonSerializer.Deserialize<List<MedicamentoEmUsoDto>>(historico.MedicamentosEmUsoJson) ?? new List<MedicamentoEmUsoDto>(); }
                catch { }
            }
            if (!string.IsNullOrEmpty(historico.DoencasCronicasJson))
            {
                try { doencasCronicas = JsonSerializer.Deserialize<List<string>>(historico.DoencasCronicasJson) ?? new List<string>(); }
                catch { }
            }
        }

        return new ResumoClinicoDto
        {
            PacienteId = pacienteId,
            TipoSanguineo = historico?.TipoSanguineo,
            QuantidadeAlergias = alergias.Count,
            AlergiasGraves = alergias.Where(a => a.Gravidade == "Grave").Select(a => a.Substancia).ToList(),
            QuantidadeMedicamentosEmUso = medicamentos.Count,
            DoencasCronicas = doencasCronicas,
            TotalConsultasRealizadas = totalConsultas,
            UltimasConsultas = ultimasConsultas.Select(c => new UltimaConsultaDto
            {
                Data = c.Data,
                Especialidade = c.Especialidade
            }).ToList()
        };
    }

    private static HistoricoClinicoDto MapearParaDto(HistoricoClinico historico)
    {
        List<AlergiaDto>? alergias = null;
        List<MedicamentoEmUsoDto>? medicamentos = null;
        List<string>? doencasCronicas = null;
        List<CirurgiaAnteriorDto>? cirurgias = null;
        List<HistoricoFamiliarDto>? historicoFamiliar = null;
        List<VacinacaoDto>? vacinacoes = null;
        HabitosSociaisDto? habitos = null;

        try
        {
            if (!string.IsNullOrEmpty(historico.AlergiasJson))
                alergias = JsonSerializer.Deserialize<List<AlergiaDto>>(historico.AlergiasJson);
            if (!string.IsNullOrEmpty(historico.MedicamentosEmUsoJson))
                medicamentos = JsonSerializer.Deserialize<List<MedicamentoEmUsoDto>>(historico.MedicamentosEmUsoJson);
            if (!string.IsNullOrEmpty(historico.DoencasCronicasJson))
                doencasCronicas = JsonSerializer.Deserialize<List<string>>(historico.DoencasCronicasJson);
            if (!string.IsNullOrEmpty(historico.CirurgiasAnterioresJson))
                cirurgias = JsonSerializer.Deserialize<List<CirurgiaAnteriorDto>>(historico.CirurgiasAnterioresJson);
            if (!string.IsNullOrEmpty(historico.HistoricoFamiliarJson))
                historicoFamiliar = JsonSerializer.Deserialize<List<HistoricoFamiliarDto>>(historico.HistoricoFamiliarJson);
            if (!string.IsNullOrEmpty(historico.VacinacoesJson))
                vacinacoes = JsonSerializer.Deserialize<List<VacinacaoDto>>(historico.VacinacoesJson);
            if (!string.IsNullOrEmpty(historico.HabitosSociaisJson))
                habitos = JsonSerializer.Deserialize<HabitosSociaisDto>(historico.HabitosSociaisJson);
        }
        catch { }

        return new HistoricoClinicoDto
        {
            Id = historico.Id,
            PacienteId = historico.PacienteId,
            Alergias = alergias,
            MedicamentosEmUso = medicamentos,
            DoencasCronicas = doencasCronicas,
            CirurgiasAnteriores = cirurgias,
            HistoricoFamiliar = historicoFamiliar,
            Vacinacoes = vacinacoes,
            HabitosSociais = habitos,
            TipoSanguineo = historico.TipoSanguineo,
            Observacoes = historico.Observacoes,
            CriadoEm = historico.CriadoEm,
            AtualizadoEm = historico.AtualizadoEm
        };
    }
}
