using MediatR;
using app.Application.Common.Models;

namespace app.Application.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQuery : IRequest<Result<int>>
{
    public int UserId { get; set; }
}
