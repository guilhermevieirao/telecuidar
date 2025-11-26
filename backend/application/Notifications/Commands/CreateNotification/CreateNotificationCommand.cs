using MediatR;
using app.Application.Common.Models;

namespace app.Application.Notifications.Commands.CreateNotification;

public class CreateNotificationCommand : IRequest<Result<int>>
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public int UserId { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
}
