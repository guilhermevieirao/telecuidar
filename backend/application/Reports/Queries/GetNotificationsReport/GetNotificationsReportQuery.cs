using MediatR;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Queries.GetNotificationsReport;

public class GetNotificationsReportQuery : IRequest<NotificationsReportDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
