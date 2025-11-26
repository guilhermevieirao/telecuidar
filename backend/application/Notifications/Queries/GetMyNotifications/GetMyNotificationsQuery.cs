using MediatR;
using app.Application.Common.Models;
using app.Application.Notifications.DTOs;

namespace app.Application.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQuery : IRequest<Result<PagedResult<NotificationDto>>>
{
    public int UserId { get; set; }
    public bool? OnlyUnread { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
