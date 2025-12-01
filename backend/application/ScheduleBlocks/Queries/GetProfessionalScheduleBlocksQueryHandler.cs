using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;
using app.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;

namespace app.Application.ScheduleBlocks.Queries;

public class GetProfessionalScheduleBlocksQueryHandler : IRequestHandler<GetProfessionalScheduleBlocksQuery, Result<List<ScheduleBlockDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetProfessionalScheduleBlocksQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ScheduleBlockDto>>> Handle(GetProfessionalScheduleBlocksQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _db.ScheduleBlocks
            .Include(b => b.Professional)
            .Where(b => b.ProfessionalId == request.ProfessionalId)
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
            AdminJustification = b.AdminJustification,
            CreatedAt = b.CreatedAt
        }).ToList();
        return Result<List<ScheduleBlockDto>>.Success(dtos);
    }
}
