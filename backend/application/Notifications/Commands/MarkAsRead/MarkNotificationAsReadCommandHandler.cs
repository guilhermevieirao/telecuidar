using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Notifications.Commands.MarkAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await _notificationRepository.GetQueryable()
                .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == request.UserId, cancellationToken);

            if (notification == null)
            {
                return Result<bool>.Failure("Notificação não encontrada");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<bool>.Success(true, "Notificação marcada como lida");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao marcar notificação como lida: {ex.Message}");
        }
    }
}
