using app.Application.Common.Interfaces;
using app.Domain.Entities;
using app.Domain.Interfaces;
using MediatR;

namespace app.Application.Users.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<AuditLog> _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTimeService;

    public DeleteAccountCommandHandler(
        IRepository<User> userRepository,
        IRepository<AuditLog> auditRepository,
        IUnitOfWork unitOfWork,
        IDateTimeService dateTimeService)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _dateTimeService = dateTimeService;
    }

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return false;
        }

        // Cria log de auditoria antes de deletar
        var auditLog = new AuditLog
        {
            UserId = user.Id,
            Action = "Exclusão de Conta (LGPD)",
            EntityName = "User",
            EntityId = user.Id,
            OldValues = $"Email: {user.Email}, Nome: {user.FirstName} {user.LastName}",
            NewValues = "Conta anonimizada por solicitação do usuário (LGPD Art. 18)",
            IpAddress = "System",
            UserAgent = "Account Deletion Request"
        };

        await _auditRepository.AddAsync(auditLog);

        // Anonimiza dados antes de deletar (conformidade LGPD)
        user.FirstName = "Usuário";
        user.LastName = "Removido";
        user.Email = $"deleted_{user.Id}@removed.local";
        user.PhoneNumber = null;
        user.ProfilePhotoUrl = null;
        user.IsActive = false;
        user.EmailConfirmed = false;
        user.PasswordHash = string.Empty;

        // Atualiza o usuário com dados anonimizados
        await _userRepository.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
