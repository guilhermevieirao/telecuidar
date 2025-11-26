using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Admin.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IRepository<AuditLog> _auditLogRepository;

    public GetAuditLogsQueryHandler(IRepository<AuditLog> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
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

            // Aplicar ordenação
            var sortBy = request.SortBy?.ToLower() ?? "createdat";
            var isDescending = request.SortDirection?.ToLower() == "desc";

            query = sortBy switch
            {
                "id" => isDescending ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
                "username" => isDescending ? query.OrderByDescending(a => a.User!.FullName) : query.OrderBy(a => a.User!.FullName),
                "action" => isDescending ? query.OrderByDescending(a => a.Action) : query.OrderBy(a => a.Action),
                "entityname" => isDescending ? query.OrderByDescending(a => a.EntityName) : query.OrderBy(a => a.EntityName),
                "createdat" => isDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt),
                _ => query.OrderByDescending(a => a.CreatedAt)
            };

            // Obter contagem total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var logs = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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

            var pagedResult = PagedResult<AuditLogDto>.Create(logDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<AuditLogDto>>.Failure($"Erro ao obter logs de auditoria: {ex.Message}");
        }
    }
}
