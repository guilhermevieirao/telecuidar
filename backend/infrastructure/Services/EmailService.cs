using app.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace app.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // TODO: Implementar com serviço real de email (SendGrid, AWS SES, etc)
    
    public Task SendPasswordResetEmailAsync(string email, string firstName, string resetToken)
    {
        // Por enquanto, apenas log
        Console.WriteLine($"[EMAIL] Password reset para {firstName} ({email}) - Token: {resetToken}");
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, string name)
    {
        Console.WriteLine($"[EMAIL] Bem-vindo {name} ({email})");
        return Task.CompletedTask;
    }

    public Task SendEmailConfirmationAsync(string email, string confirmationToken)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
        var confirmationLink = $"{frontendUrl}/confirm-email?token={confirmationToken}";
        
        Console.WriteLine($"========================================");
        Console.WriteLine($"[EMAIL] Confirmação de Email");
        Console.WriteLine($"Para: {email}");
        Console.WriteLine($"Link de confirmação: {confirmationLink}");
        Console.WriteLine($"Token: {confirmationToken}");
        Console.WriteLine($"========================================");
        
        return Task.CompletedTask;
    }

    public Task SendInvitationEmailAsync(string email, string roleName, string invitationLink)
    {
        Console.WriteLine($"========================================");
        Console.WriteLine($"[EMAIL] Convite para Cadastro");
        Console.WriteLine($"Para: {email}");
        Console.WriteLine($"Perfil: {roleName}");
        Console.WriteLine($"Link de convite: {invitationLink}");
        Console.WriteLine($"========================================");
        
        return Task.CompletedTask;
    }
}
