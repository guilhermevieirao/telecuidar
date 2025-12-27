using Application.DTOs.Especialidades;

namespace Application.Interfaces;

/// <summary>
/// Serviço de especialidades
/// </summary>
public interface IEspecialidadeService
{
    Task<EspecialidadesPaginadasDto> ObterEspecialidadesAsync(int pagina, int tamanhoPagina, string? busca, string? status);
    Task<List<EspecialidadeDto>> ObterTodasEspecialidadesAtivasAsync();
    Task<EspecialidadeDto?> ObterEspecialidadePorIdAsync(Guid id);
    Task<EspecialidadeDto> CriarEspecialidadeAsync(CriarEspecialidadeDto dto);
    Task<EspecialidadeDto?> AtualizarEspecialidadeAsync(Guid id, AtualizarEspecialidadeDto dto);
    Task<bool> DeletarEspecialidadeAsync(Guid id);
    Task<bool> AtivarEspecialidadeAsync(Guid id);
    Task<bool> DesativarEspecialidadeAsync(Guid id);
    Task<List<CampoPersonalizadoDto>?> ObterCamposPersonalizadosAsync(Guid id);
}
