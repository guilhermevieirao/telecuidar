using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

/// <summary>
/// Serviço para envio de e-mails usando MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _frontendUrl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string corpo, bool html = true)
    {
        try
        {
            var smtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? _configuration["EmailSettings:SmtpHost"];
            var smtpPortStr = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? _configuration["EmailSettings:SmtpPort"];
            var smtpUser = Environment.GetEnvironmentVariable("EMAIL_SMTP_USER") ?? _configuration["EmailSettings:SmtpUser"];
            var smtpPass = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? _configuration["EmailSettings:FromAddress"] ?? "noreply@telecuidar.com";
            var fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? _configuration["EmailSettings:FromName"] ?? "TeleCuidar";

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
            {
                _logger.LogWarning("Configuração SMTP não encontrada. E-mail não será enviado.");
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(destinatario, destinatario));
            message.Subject = assunto;

            var bodyBuilder = new BodyBuilder();
            if (html)
            {
                bodyBuilder.HtmlBody = corpo;
            }
            else
            {
                bodyBuilder.TextBody = corpo;
            }
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            var port = int.Parse(smtpPortStr ?? "587");
            
            // Determinar o tipo de conexão SSL baseado na porta
            // Porta 465: SSL direto (SslOnConnect)
            // Porta 587: STARTTLS (StartTls)
            // Outras portas: verificar EMAIL_USE_SSL
            SecureSocketOptions secureOptions;
            if (port == 465)
            {
                secureOptions = SecureSocketOptions.SslOnConnect;
            }
            else if (port == 587)
            {
                secureOptions = SecureSocketOptions.StartTls;
            }
            else
            {
                var useSslStr = Environment.GetEnvironmentVariable("EMAIL_USE_SSL") ?? _configuration["EmailSettings:UseSsl"];
                var useSsl = useSslStr?.ToLower() != "false";
                secureOptions = useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            }
            
            await client.ConnectAsync(smtpHost, port, secureOptions);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("E-mail enviado para {Destinatario}", destinatario);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail para {Destinatario}", destinatario);
            return false;
        }
    }

    public async Task<bool> EnviarEmailVerificacaoAsync(Guid usuarioId, string email, string nome, string token)
    {
        var html = EmailTemplateService.GerarEmailVerificacaoHtml(nome, token, _frontendUrl);
        return await EnviarEmailAsync(email, "[TeleCuidar] Confirme seu E-mail", html);
    }

    public async Task<bool> EnviarEmailRedefinicaoSenhaAsync(string email, string nome, string token)
    {
        var html = EmailTemplateService.GerarEmailRedefinicaoSenhaHtml(nome, token, _frontendUrl);
        return await EnviarEmailAsync(email, "[TeleCuidar] Redefinição de Senha", html);
    }

    public async Task<bool> EnviarEmailConviteAsync(string email, string tokenConvite, string tipoUsuario, string? nomeConvidador)
    {
        var html = EmailTemplateService.GerarEmailConviteHtml(tipoUsuario, tokenConvite, nomeConvidador ?? "", _frontendUrl);
        return await EnviarEmailAsync(email, "[TeleCuidar] Você foi convidado!", html);
    }

    public async Task<bool> EnviarEmailConfirmacaoConsultaAsync(string emailPaciente, string nomePaciente, string nomeProfissional, string especialidade, DateTime dataConsulta, string horario)
    {
        var html = EmailTemplateService.GerarEmailConfirmacaoConsultaHtml(nomePaciente, nomeProfissional, especialidade, dataConsulta, horario);
        return await EnviarEmailAsync(emailPaciente, "[TeleCuidar] Consulta Confirmada", html);
    }

    public async Task<bool> EnviarEmailLembreteConsultaAsync(string email, string nome, string nomeProfissional, DateTime dataConsulta, string horario, string? linkConsulta)
    {
        var html = EmailTemplateService.GerarEmailLembreteConsultaHtml(nome, nomeProfissional, dataConsulta, horario, linkConsulta);
        return await EnviarEmailAsync(email, "[TeleCuidar] Lembrete de Consulta", html);
    }

    public async Task<bool> EnviarEmailTrocaEmailAsync(string novoEmail, string nome, string token)
    {
        var html = EmailTemplateService.GerarEmailTrocaEmailHtml(nome, token, _frontendUrl);
        return await EnviarEmailAsync(novoEmail, "[TeleCuidar] Confirme seu Novo E-mail", html);
    }
}
