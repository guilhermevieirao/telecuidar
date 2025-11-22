using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;

namespace app.Application.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<Result<List<AuditLog>>>
{
    public Guid? UserId { get; set; }
    public string? Action { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
