using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Enums;

namespace app.Application.Admin.Commands.CreateInvitation;

public class CreateInvitationCommand : IRequest<Result<InvitationTokenDto>>
{
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int CreatedByUserId { get; set; }
    
    // Campos opcionais para pré-preencher
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public int ExpirationHours { get; set; } = 168; // 7 dias padrão
}
