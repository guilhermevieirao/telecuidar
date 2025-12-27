using Application.DTOs.Cns;

namespace Application.Interfaces;

/// <summary>
/// Serviço de CNS (Cartão Nacional de Saúde)
/// </summary>
public interface ICnsService
{
    ValidarCnsResponseDto ValidarCns(string cns);
    Task<InfoCnsResponseDto?> BuscarInfoCnsAsync(string cns);
}
