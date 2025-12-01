using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;
using app.Domain.Entities;
using app.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;

namespace app.Application.ScheduleBlocks.Commands;

public class AdminRespondScheduleBlockCommandHandler : IRequestHandler<AdminRespondScheduleBlockCommand, Result<ScheduleBlockDto>>
{
    private readonly IApplicationDbContext _db;
    public AdminRespondScheduleBlockCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ScheduleBlockDto>> Handle(AdminRespondScheduleBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _db.ScheduleBlocks.Include(b => b.Professional).FirstOrDefaultAsync(b => b.Id == request.BlockId, cancellationToken);
        if (block == null)
            return Result<ScheduleBlockDto>.Failure("Bloqueio não encontrado.");
        if (block.Status != BlockStatus.Pending)
            return Result<ScheduleBlockDto>.Failure("Bloqueio já foi respondido.");

        block.Status = request.Accept ? BlockStatus.Accepted : BlockStatus.Rejected;
        block.AdminId = request.AdminId;
        block.AdminJustification = request.Justification;
        block.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new ScheduleBlockDto
        {
            Id = block.Id,
            ProfessionalId = block.ProfessionalId,
            ProfessionalName = block.Professional.FullName,
            StartDate = block.StartDate,
            EndDate = block.EndDate,
            Reason = block.Reason,
            Status = block.Status,
            AdminId = block.AdminId,
            AdminJustification = block.AdminJustification,
            CreatedAt = block.CreatedAt
        };
        return Result<ScheduleBlockDto>.Success(dto);
    }
}
