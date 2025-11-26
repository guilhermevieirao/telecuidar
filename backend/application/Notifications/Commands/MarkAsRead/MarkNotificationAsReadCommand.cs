using MediatR;
using app.Application.Common.Models;

namespace app.Application.Notifications.Commands.MarkAsRead;

public class MarkNotificationAsReadCommand : IRequest<Result<bool>>
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
}
