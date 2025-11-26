using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Reports.DTOs;
using app.Domain.Interfaces;
using app.Domain.Entities;

namespace app.Application.Reports.Queries.GetNotificationsReport;

public class GetNotificationsReportQueryHandler : IRequestHandler<GetNotificationsReportQuery, NotificationsReportDto>
{
    private readonly IRepository<Notification> _notificationRepository;

    public GetNotificationsReportQueryHandler(IRepository<Notification> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationsReportDto> Handle(GetNotificationsReportQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.Now.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.Now;

        var query = _notificationRepository.GetQueryable()
            .Where(n => n.CreatedAt >= startDate && n.CreatedAt <= endDate)
            .Include(n => n.User);

        var notifications = await query.ToListAsync(cancellationToken);

        var totalNotifications = notifications.Count;
        var readNotifications = notifications.Count(n => n.IsRead);
        var unreadNotifications = totalNotifications - readNotifications;
        var readPercentage = totalNotifications > 0 ? (double)readNotifications / totalNotifications * 100 : 0;

        var report = new NotificationsReportDto
        {
            TotalNotifications = totalNotifications,
            ReadNotifications = readNotifications,
            UnreadNotifications = unreadNotifications,
            ReadPercentage = Math.Round(readPercentage, 2),
            StartDate = startDate,
            EndDate = endDate,
            GeneratedAt = DateTime.Now,
            TypeCounts = notifications.GroupBy(n => n.Type)
                .ToDictionary(g => g.Key, g => g.Count()),
            NotificationDetails = notifications.Select(n => new NotificationDetailDto
            {
                Id = n.Id,
                Title = n.Title,
                Type = n.Type,
                UserName = n.User.FullName,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).OrderByDescending(n => n.CreatedAt).ToList()
        };

        return report;
    }
}
