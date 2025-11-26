using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Notifications.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IRepository<Notification> _notificationRepository;

    public GetMyNotificationsQueryHandler(IRepository<Notification> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<PagedResult<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _notificationRepository.GetQueryable()
                .Include(n => n.CreatedByUser)
                .Where(n => n.UserId == request.UserId);

            // Filtrar apenas não lidas se solicitado
            if (request.OnlyUnread == true)
            {
                query = query.Where(n => !n.IsRead);
            }

            // Ordenar por data (mais recentes primeiro)
            query = query.OrderByDescending(n => n.CreatedAt);

            // Obter contagem total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var notifications = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ActionUrl = n.ActionUrl,
                ActionText = n.ActionText,
                UserId = n.UserId,
                CreatedByUserId = n.CreatedByUserId,
                CreatedByUserName = n.CreatedByUser?.FullName,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt,
                TimeAgo = FormatTimeAgo(n.CreatedAt)
            }).ToList();

            var pagedResult = PagedResult<NotificationDto>.Create(notificationDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PagedResult<NotificationDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<NotificationDto>>.Failure($"Erro ao buscar notificações: {ex.Message}");
        }
    }

    private static string FormatTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Agora";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}m atrás";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}h atrás";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}d atrás";
        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)}sem atrás";
        
        return dateTime.ToString("dd/MM/yyyy");
    }
}
