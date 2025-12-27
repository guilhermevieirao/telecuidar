using Application.DTOs.Medicamentos;

namespace Application.Interfaces;

/// <summary>
/// Serviço de consulta de medicamentos ANVISA
/// </summary>
public interface IMedicamentoAnvisaService
{
    /// <summary>
    /// Busca medicamentos por termo com paginação
    /// </summary>
    Task<MedicamentosPaginadosDto> BuscarMedicamentosAsync(string termo, int pagina = 1, int tamanhoPagina = 20);

    /// <summary>
    /// Obtém um medicamento pelo número de registro
    /// </summary>
    Task<MedicamentoAnvisaDto?> ObterMedicamentoPorRegistroAsync(string numeroRegistro);

    /// <summary>
    /// Obtém lista de princípios ativos
    /// </summary>
    Task<List<string>> ObterPrincipiosAtivosAsync(string termo);

    /// <summary>
    /// Busca medicamentos por princípio ativo
    /// </summary>
    Task<List<MedicamentoAnvisaDto>> BuscarPorPrincipioAtivoAsync(string principioAtivo);
}
