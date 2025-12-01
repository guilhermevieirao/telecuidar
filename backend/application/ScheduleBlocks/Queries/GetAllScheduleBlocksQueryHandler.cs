using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;
using app.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;

namespace app.Application.ScheduleBlocks.Queries;

public class GetAllScheduleBlocksQueryHandler : IRequestHandler<GetAllScheduleBlocksQuery, Result<List<ScheduleBlockDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetAllScheduleBlocksQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ScheduleBlockDto>>> Handle(GetAllScheduleBlocksQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _db.ScheduleBlocks
            .Include(b => b.Professional)
            .Include(b => b.Admin)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = blocks.Select(b => new ScheduleBlockDto
        {
            Id = b.Id,
            ProfessionalId = b.ProfessionalId,
            ProfessionalName = b.Professional.FullName,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Reason = b.Reason,
            Status = b.Status,
            AdminId = b.AdminId,
            AdminName = b.Admin != null ? b.Admin.FullName : null,
            AdminJustification = b.AdminJustification,
            CreatedAt = b.CreatedAt
        }).ToList();
        return Result<List<ScheduleBlockDto>>.Success(dtos);
    }
}
