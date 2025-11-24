namespace app.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string firstName, string resetToken);
    Task SendWelcomeEmailAsync(string email, string name);
    Task SendEmailConfirmationAsync(string email, string confirmationToken);
    Task SendInvitationEmailAsync(string email, string roleName, string invitationLink);
}
