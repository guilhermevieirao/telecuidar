using Application.DTOs.Especialidades;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de especialidades
/// </summary>
public class EspecialidadeService : IEspecialidadeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EspecialidadeService> _logger;

    public EspecialidadeService(ApplicationDbContext context, ILogger<EspecialidadeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EspecialidadesPaginadasDto> ObterEspecialidadesAsync(int pagina, int tamanhoPagina, string? busca, string? status)
    {
        var query = _context.Especialidades.AsQueryable();

        if (!string.IsNullOrEmpty(busca))
        {
            busca = busca.ToLower();
            query = query.Where(e => e.Nome.ToLower().Contains(busca) || e.Descricao.ToLower().Contains(busca));
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusEspecialidade>(status, true, out var statusEnum))
        {
            query = query.Where(e => e.Status == statusEnum);
        }

        var total = await query.CountAsync();
        var especialidades = await query
            .OrderBy(e => e.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new EspecialidadesPaginadasDto
        {
            Dados = especialidades.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<List<EspecialidadeDto>> ObterTodasEspecialidadesAtivasAsync()
    {
        var especialidades = await _context.Especialidades
            .Where(e => e.Status == StatusEspecialidade.Ativa)
            .OrderBy(e => e.Nome)
            .ToListAsync();

        return especialidades.Select(MapearParaDto).ToList();
    }

    public async Task<EspecialidadeDto?> ObterEspecialidadePorIdAsync(Guid id)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        return especialidade != null ? MapearParaDto(especialidade) : null;
    }

    public async Task<EspecialidadeDto> CriarEspecialidadeAsync(CriarEspecialidadeDto dto)
    {
        if (await _context.Especialidades.AnyAsync(e => e.Nome.ToLower() == dto.Nome.ToLower()))
        {
            throw new InvalidOperationException("Já existe uma especialidade com este nome.");
        }

        var especialidade = new Especialidade
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            CamposPersonalizadosJson = dto.CamposPersonalizadosJson,
            Status = StatusEspecialidade.Ativa,
            CriadoEm = DateTime.UtcNow
        };

        _context.Especialidades.Add(especialidade);
        await _context.SaveChangesAsync();

        return MapearParaDto(especialidade);
    }

    public async Task<EspecialidadeDto?> AtualizarEspecialidadeAsync(Guid id, AtualizarEspecialidadeDto dto)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        if (especialidade == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Nome))
        {
            if (await _context.Especialidades.AnyAsync(e => e.Nome.ToLower() == dto.Nome.ToLower() && e.Id != id))
            {
                throw new InvalidOperationException("Já existe uma especialidade com este nome.");
            }
            especialidade.Nome = dto.Nome;
        }

        if (!string.IsNullOrEmpty(dto.Descricao))
        {
            especialidade.Descricao = dto.Descricao;
        }

        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<StatusEspecialidade>(dto.Status, true, out var statusEnum))
        {
            especialidade.Status = statusEnum;
        }

        if (dto.CamposPersonalizadosJson != null)
        {
            especialidade.CamposPersonalizadosJson = dto.CamposPersonalizadosJson;
        }

        especialidade.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapearParaDto(especialidade);
    }

    public async Task<bool> DeletarEspecialidadeAsync(Guid id)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        if (especialidade == null)
        {
            return false;
        }

        // Verificar se há profissionais vinculados
        var temProfissionais = await _context.PerfisProfissional.AnyAsync(p => p.EspecialidadeId == id);
        if (temProfissionais)
        {
            throw new InvalidOperationException("Não é possível excluir uma especialidade com profissionais vinculados.");
        }

        _context.Especialidades.Remove(especialidade);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AtivarEspecialidadeAsync(Guid id)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        if (especialidade == null)
        {
            return false;
        }

        especialidade.Status = StatusEspecialidade.Ativa;
        especialidade.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesativarEspecialidadeAsync(Guid id)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        if (especialidade == null)
        {
            return false;
        }

        especialidade.Status = StatusEspecialidade.Inativa;
        especialidade.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CampoPersonalizadoDto>?> ObterCamposPersonalizadosAsync(Guid id)
    {
        var especialidade = await _context.Especialidades.FindAsync(id);
        if (especialidade == null || string.IsNullOrEmpty(especialidade.CamposPersonalizadosJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<CampoPersonalizadoDto>>(especialidade.CamposPersonalizadosJson);
        }
        catch
        {
            return null;
        }
    }

    private static EspecialidadeDto MapearParaDto(Especialidade especialidade)
    {
        return new EspecialidadeDto
        {
            Id = especialidade.Id,
            Nome = especialidade.Nome,
            Descricao = especialidade.Descricao,
            Status = especialidade.Status.ToString(),
            CamposPersonalizadosJson = especialidade.CamposPersonalizadosJson,
            CriadoEm = especialidade.CriadoEm,
            AtualizadoEm = especialidade.AtualizadoEm
        };
    }
}
