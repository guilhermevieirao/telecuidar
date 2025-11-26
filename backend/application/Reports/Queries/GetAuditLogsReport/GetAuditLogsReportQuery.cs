using MediatR;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Queries.GetAuditLogsReport;

public class GetAuditLogsReportQuery : IRequest<AuditLogsReportDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
