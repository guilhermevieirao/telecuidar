using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly IRepository<Notification> _notificationRepository;

    public GetUnreadCountQueryHandler(IRepository<Notification> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var count = await _notificationRepository.GetQueryable()
                .CountAsync(n => n.UserId == request.UserId && !n.IsRead, cancellationToken);

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Erro ao buscar contagem de notificações: {ex.Message}");
        }
    }
}
