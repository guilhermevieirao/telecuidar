namespace Application.Interfaces;

/// <summary>
/// Serviço de hash de senha
/// </summary>
public interface IHashSenhaService
{
    string GerarHash(string senha);
    bool VerificarHash(string senha, string hash);
}
