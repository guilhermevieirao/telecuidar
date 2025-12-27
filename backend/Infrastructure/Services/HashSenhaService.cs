using Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Infrastructure.Services;

/// <summary>
/// Serviço para hash de senhas usando BCrypt
/// </summary>
public class HashSenhaService : IHashSenhaService
{
    private const int WorkFactor = 12;

    public string GerarHash(string senha)
    {
        return BC.HashPassword(senha, workFactor: WorkFactor);
    }

    public bool VerificarHash(string senha, string hash)
    {
        try
        {
            return BC.Verify(senha, hash);
        }
        catch
        {
            return false;
        }
    }
}
