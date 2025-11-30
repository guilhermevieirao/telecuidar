using app.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace app.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string? _smtpHost;
    private readonly int _smtpPort;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;
    private readonly string? _fromEmail;
    private readonly string? _fromName;
    private readonly bool _enableSsl;
    private readonly bool _isConfigured;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        try
        {
            _smtpHost = _configuration["Email:SmtpHost"];
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:SmtpUsername"];
            _smtpPassword = _configuration["Email:SmtpPassword"];
            _fromEmail = _configuration["Email:FromEmail"] ?? _smtpUsername;
            _fromName = _configuration["Email:FromName"] ?? "Telecuidar";
            _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
            
            _isConfigured = !string.IsNullOrEmpty(_smtpHost) && 
                           !string.IsNullOrEmpty(_smtpUsername) && 
                           !string.IsNullOrEmpty(_smtpPassword);
            
            if (!_isConfigured)
            {
                Console.WriteLine("⚠️  [EMAIL] Configurações SMTP incompletas - emails serão apenas logados no console");
            }
            else
            {
                Console.WriteLine($"✅ [EMAIL] SMTP configurado: {_smtpHost}:{_smtpPort}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  [EMAIL] Erro ao carregar configurações: {ex.Message}");
            _isConfigured = false;
        }
    }
    
    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        // Se não está configurado, apenas loga no console
        if (!_isConfigured || string.IsNullOrEmpty(_smtpHost) || string.IsNullOrEmpty(_smtpUsername))
        {
            Console.WriteLine($"📧 [EMAIL SIMULADO] Para: {toEmail}");
            Console.WriteLine($"   Assunto: {subject}");
            Console.WriteLine($"   (Configure SMTP no .env para enviar emails reais)");
            return;
        }
        
        try
        {
            Console.WriteLine($"📤 [EMAIL] Tentando enviar para: {toEmail}");
            Console.WriteLine($"   SMTP: {_smtpHost}:{_smtpPort} (SSL: {_enableSsl})");
            
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail!, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            
            message.To.Add(new MailAddress(toEmail));
            
            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 30000 // 30 segundos de timeout
            };
            
            await smtpClient.SendMailAsync(message);
            Console.WriteLine($"✅ [EMAIL ENVIADO] Para: {toEmail} - Assunto: {subject}");
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ [ERRO SMTP] Para: {toEmail}");
            Console.WriteLine($"   Código: {smtpEx.StatusCode}");
            Console.WriteLine($"   Mensagem: {smtpEx.Message}");
            if (smtpEx.InnerException != null)
            {
                Console.WriteLine($"   Detalhe: {smtpEx.InnerException.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [ERRO EMAIL] Para: {toEmail}");
            Console.WriteLine($"   Tipo: {ex.GetType().Name}");
            Console.WriteLine($"   Mensagem: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Detalhe: {ex.InnerException.Message}");
            }
        }
    }
    
    public async Task SendPasswordResetEmailAsync(string email, string firstName, string resetToken)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
        var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
        
        var htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #059669;'>Redefinição de Senha</h2>
                    <p>Olá {firstName},</p>
                    <p>Recebemos uma solicitação para redefinir a senha da sua conta no Telecuidar.</p>
                    <p>Para redefinir sua senha, clique no botão abaixo:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='background-color: #059669; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Redefinir Senha</a>
                    </p>
                    <p>Ou copie e cole o seguinte link no seu navegador:</p>
                    <p style='background-color: #f3f4f6; padding: 10px; border-radius: 5px; word-break: break-all;'>{resetLink}</p>
                    <p><strong>Este link é válido por 1 hora.</strong></p>
                    <p>Se você não solicitou a redefinição de senha, ignore este email.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #6b7280;'>Telecuidar - Telemedicina e Cuidados à Distância</p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, "Redefinição de Senha - Telecuidar", htmlBody);
    }

    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        var htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #059669;'>Bem-vindo ao Telecuidar!</h2>
                    <p>Olá {name},</p>
                    <p>É um prazer tê-lo(a) conosco!</p>
                    <p>O Telecuidar é sua plataforma de telemedicina e cuidados à distância, oferecendo consultas online, prontuários eletrônicos e muito mais.</p>
                    <p>Aproveite todos os recursos disponíveis para cuidar melhor da sua saúde.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #6b7280;'>Telecuidar - Telemedicina e Cuidados à Distância</p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, "Bem-vindo ao Telecuidar!", htmlBody);
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationToken)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
        var confirmationLink = $"{frontendUrl}/confirm-email?token={confirmationToken}";
        
        var htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #059669;'>Confirme seu Email</h2>
                    <p>Obrigado por se cadastrar no Telecuidar!</p>
                    <p>Para ativar sua conta, clique no botão abaixo para confirmar seu endereço de email:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' style='background-color: #059669; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Confirmar Email</a>
                    </p>
                    <p>Ou copie e cole o seguinte link no seu navegador:</p>
                    <p style='background-color: #f3f4f6; padding: 10px; border-radius: 5px; word-break: break-all;'>{confirmationLink}</p>
                    <p><strong>Este link é válido por 24 horas.</strong></p>
                    <p>Se você não se cadastrou no Telecuidar, ignore este email.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #6b7280;'>Telecuidar - Telemedicina e Cuidados à Distância</p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, "Confirme seu Email - Telecuidar", htmlBody);
    }

    public async Task SendInvitationEmailAsync(string email, string roleName, string invitationLink)
    {
        var htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #059669;'>Você foi convidado para o Telecuidar!</h2>
                    <p>Você recebeu um convite para se cadastrar no Telecuidar como <strong>{roleName}</strong>.</p>
                    <p>Para completar seu cadastro, clique no botão abaixo:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{invitationLink}' style='background-color: #059669; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Aceitar Convite</a>
                    </p>
                    <p>Ou copie e cole o seguinte link no seu navegador:</p>
                    <p style='background-color: #f3f4f6; padding: 10px; border-radius: 5px; word-break: break-all;'>{invitationLink}</p>
                    <p><strong>Este convite é válido por 7 dias.</strong></p>
                    <p>Após completar o cadastro, você terá acesso completo à plataforma.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #6b7280;'>Telecuidar - Telemedicina e Cuidados à Distância</p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, $"Convite para {roleName} - Telecuidar", htmlBody);
    }
}
