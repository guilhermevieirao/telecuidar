using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Admin.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<List<AuditLogDto>>>
{
    private readonly IRepository<AuditLog> _auditLogRepository;

    public GetAuditLogsQueryHandler(IRepository<AuditLog> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _auditLogRepository.GetQueryable()
                .Include(a => a.User)
                .AsQueryable();

            if (request.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == request.UserId.Value);
            }

            if (!string.IsNullOrEmpty(request.EntityName))
            {
                query = query.Where(a => a.EntityName == request.EntityName);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= request.EndDate.Value);
            }

            query = query.OrderByDescending(a => a.CreatedAt);

            if (request.Limit.HasValue)
            {
                query = query.Take(request.Limit.Value);
            }

            var logs = await query.ToListAsync(cancellationToken);

            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User != null ? log.User.FullName : null,
                UserEmail = log.User?.Email,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                OldValues = log.OldValues,
                NewValues = log.NewValues,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent,
                CreatedAt = log.CreatedAt
            }).ToList();

            return Result<List<AuditLogDto>>.Success(logDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AuditLogDto>>.Failure($"Erro ao obter logs de auditoria: {ex.Message}");
        }
    }
}
