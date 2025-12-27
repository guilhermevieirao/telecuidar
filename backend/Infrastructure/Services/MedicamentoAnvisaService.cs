using Application.DTOs.Medicamentos;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de consulta de medicamentos ANVISA
/// </summary>
public class MedicamentoAnvisaService : IMedicamentoAnvisaService
{
    private readonly ILogger<MedicamentoAnvisaService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _caminhoBaseAnvisa;

    public MedicamentoAnvisaService(
        ILogger<MedicamentoAnvisaService> logger, 
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("Anvisa");
        _cache = cache;
        _caminhoBaseAnvisa = Path.Combine(Directory.GetCurrentDirectory(), "Data", "anvisa");
    }

    public async Task<MedicamentosPaginadosDto> BuscarMedicamentosAsync(string termo, int pagina = 1, int tamanhoPagina = 20)
    {
        var cacheKey = $"medicamentos_{termo}_{pagina}_{tamanhoPagina}";
        
        if (_cache.TryGetValue(cacheKey, out MedicamentosPaginadosDto? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            // Tentar buscar do arquivo local primeiro
            var medicamentos = await BuscarLocalAsync(termo);

            if (medicamentos.Any())
            {
                var total = medicamentos.Count;
                var paginados = medicamentos
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();

                var resultado = new MedicamentosPaginadosDto
                {
                    Dados = paginados,
                    Total = total,
                    Pagina = pagina,
                    TamanhoPagina = tamanhoPagina,
                    TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
                };

                // Cache por 1 hora
                _cache.Set(cacheKey, resultado, TimeSpan.FromHours(1));
                return resultado;
            }

            return new MedicamentosPaginadosDto
            {
                Dados = new List<MedicamentoAnvisaDto>(),
                Total = 0,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalPaginas = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar medicamentos ANVISA");
            return new MedicamentosPaginadosDto
            {
                Dados = new List<MedicamentoAnvisaDto>(),
                Total = 0,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalPaginas = 0
            };
        }
    }

    public async Task<MedicamentoAnvisaDto?> ObterMedicamentoPorRegistroAsync(string numeroRegistro)
    {
        var cacheKey = $"medicamento_{numeroRegistro}";

        if (_cache.TryGetValue(cacheKey, out MedicamentoAnvisaDto? cached))
        {
            return cached;
        }

        try
        {
            var medicamentos = await BuscarLocalAsync(numeroRegistro);
            var medicamento = medicamentos.FirstOrDefault(m => m.NumeroRegistro == numeroRegistro);

            if (medicamento != null)
            {
                _cache.Set(cacheKey, medicamento, TimeSpan.FromHours(24));
            }

            return medicamento;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter medicamento por registro");
            return null;
        }
    }

    public async Task<List<string>> ObterPrincipiosAtivosAsync(string termo)
    {
        var cacheKey = $"principios_ativos_{termo}";

        if (_cache.TryGetValue(cacheKey, out List<string>? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            var medicamentos = await BuscarLocalAsync(termo);
            var principios = medicamentos
                .Where(m => !string.IsNullOrEmpty(m.PrincipioAtivo))
                .Select(m => m.PrincipioAtivo!)
                .Distinct()
                .Take(20)
                .ToList();

            _cache.Set(cacheKey, principios, TimeSpan.FromHours(1));
            return principios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter princípios ativos");
            return new List<string>();
        }
    }

    public async Task<List<MedicamentoAnvisaDto>> BuscarPorPrincipioAtivoAsync(string principioAtivo)
    {
        var cacheKey = $"por_principio_{principioAtivo}";

        if (_cache.TryGetValue(cacheKey, out List<MedicamentoAnvisaDto>? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            var medicamentos = await BuscarLocalAsync(principioAtivo);
            var filtrados = medicamentos
                .Where(m => m.PrincipioAtivo?.Contains(principioAtivo, StringComparison.OrdinalIgnoreCase) == true)
                .Take(50)
                .ToList();

            _cache.Set(cacheKey, filtrados, TimeSpan.FromHours(1));
            return filtrados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar por princípio ativo");
            return new List<MedicamentoAnvisaDto>();
        }
    }

    private async Task<List<MedicamentoAnvisaDto>> BuscarLocalAsync(string termo)
    {
        var resultado = new List<MedicamentoAnvisaDto>();
        var arquivoJson = Path.Combine(_caminhoBaseAnvisa, "medicamentos.json");

        if (!File.Exists(arquivoJson))
        {
            _logger.LogWarning("Arquivo de medicamentos ANVISA não encontrado: {Arquivo}", arquivoJson);
            return resultado;
        }

        try
        {
            var json = await File.ReadAllTextAsync(arquivoJson);
            var medicamentos = JsonSerializer.Deserialize<List<MedicamentoAnvisaDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (medicamentos != null)
            {
                var termoNormalizado = NormalizarTexto(termo);
                resultado = medicamentos
                    .Where(m => 
                        NormalizarTexto(m.NomeComercial ?? "").Contains(termoNormalizado) ||
                        NormalizarTexto(m.PrincipioAtivo ?? "").Contains(termoNormalizado) ||
                        NormalizarTexto(m.Laboratorio ?? "").Contains(termoNormalizado) ||
                        (m.NumeroRegistro ?? "").Contains(termo))
                    .Take(100)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ler arquivo de medicamentos");
        }

        return resultado;
    }

    private static string NormalizarTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        var normalizado = texto.ToLowerInvariant();
        normalizado = new string(normalizado
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        return normalizado;
    }
}
