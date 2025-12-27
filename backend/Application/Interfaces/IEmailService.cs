using Application.DTOs.Email;

namespace Application.Interfaces;

/// <summary>
/// Serviço de email
/// </summary>
public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string assunto, string corpo, bool html = true);
    Task<bool> EnviarEmailVerificacaoAsync(Guid usuarioId, string email, string nome, string token);
    Task<bool> EnviarEmailRedefinicaoSenhaAsync(string email, string nome, string token);
    Task<bool> EnviarEmailConviteAsync(string email, string tokenConvite, string tipoUsuario, string? nomeConvidador);
    Task<bool> EnviarEmailConfirmacaoConsultaAsync(string emailPaciente, string nomePaciente, string nomeProfissional, string especialidade, DateTime dataConsulta, string horario);
    Task<bool> EnviarEmailLembreteConsultaAsync(string email, string nome, string nomeProfissional, DateTime dataConsulta, string horario, string? linkConsulta);
    Task<bool> EnviarEmailTrocaEmailAsync(string novoEmail, string nome, string token);
}
