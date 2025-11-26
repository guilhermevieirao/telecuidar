using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;

namespace app.Application.Admin.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<Result<PagedResult<AuditLogDto>>>
{
    public int? UserId { get; set; }
    public string? EntityName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}
