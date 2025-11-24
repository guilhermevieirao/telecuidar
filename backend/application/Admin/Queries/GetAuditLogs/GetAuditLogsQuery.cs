using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;

namespace app.Application.Admin.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<Result<List<AuditLogDto>>>
{
    public int? UserId { get; set; }
    public string? EntityName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Limit { get; set; } = 100;
}
