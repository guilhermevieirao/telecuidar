using app.Application.Cadsus.DTOs;

namespace app.Application.Common.Interfaces;

public interface ICadsusService
{
    Task<CadsusTokenStatus> GetTokenStatusAsync();
    Task<string> RenewTokenAsync();
    Task<CadsusCidadao> ConsultarCpfAsync(string cpf);
}
