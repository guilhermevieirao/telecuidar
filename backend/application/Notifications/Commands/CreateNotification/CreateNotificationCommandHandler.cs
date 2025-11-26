using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Notifications.Commands.CreateNotification;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<int>>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationCommandHandler(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = new Notification
            {
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                ActionUrl = request.ActionUrl,
                ActionText = request.ActionText,
                UserId = request.UserId,
                CreatedByUserId = request.CreatedByUserId,
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(notification.Id, "Notificação criada com sucesso");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Erro ao criar notificação: {ex.Message}");
        }
    }
}
