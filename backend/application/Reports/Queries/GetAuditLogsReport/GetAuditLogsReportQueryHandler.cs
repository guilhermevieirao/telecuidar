using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Reports.DTOs;
using app.Domain.Interfaces;
using app.Domain.Entities;

namespace app.Application.Reports.Queries.GetAuditLogsReport;

public class GetAuditLogsReportQueryHandler : IRequestHandler<GetAuditLogsReportQuery, AuditLogsReportDto>
{
    private readonly IRepository<AuditLog> _auditLogRepository;

    public GetAuditLogsReportQueryHandler(IRepository<AuditLog> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogsReportDto> Handle(GetAuditLogsReportQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.Now.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.Now;

        var query = _auditLogRepository.GetQueryable()
            .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate);

        var logs = await query.Include(a => a.User).ToListAsync(cancellationToken);

        var report = new AuditLogsReportDto
        {
            TotalLogs = logs.Count,
            StartDate = startDate,
            EndDate = endDate,
            GeneratedAt = DateTime.Now,
            ActionCounts = logs.GroupBy(l => l.Action)
                .ToDictionary(g => g.Key, g => g.Count()),
            EntityCounts = logs.GroupBy(l => l.EntityName)
                .ToDictionary(g => g.Key, g => g.Count()),
            LogDetails = logs.Select(l => new AuditLogDetailDto
            {
                Id = l.Id,
                UserName = l.User?.FullName ?? "Sistema",
                Action = l.Action,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                IpAddress = l.IpAddress,
                CreatedAt = l.CreatedAt
            }).OrderByDescending(l => l.CreatedAt).ToList()
        };

        return report;
    }
}
