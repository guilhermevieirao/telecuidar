using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<List<AuditLog>>>
{
    private readonly IRepository<AuditLog> _auditLogRepository;

    public GetAuditLogsQueryHandler(IRepository<AuditLog> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<List<AuditLog>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _auditLogRepository.GetQueryable();

            if (request.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == request.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Action))
            {
                query = query.Where(a => a.Action.ToString().Contains(request.Action));
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= request.EndDate.Value);
            }

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Take(100)
                .ToListAsync(cancellationToken);

            return Result<List<AuditLog>>.Success(logs);
        }
        catch (Exception ex)
        {
            return Result<List<AuditLog>>.Failure("Erro ao buscar logs de auditoria", new List<string> { ex.Message });
        }
    }
}
