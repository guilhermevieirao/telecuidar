using app.Application.Common.Models;
using app.Application.ScheduleBlocks.DTOs;
using app.Domain.Entities;
using app.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;

namespace app.Application.ScheduleBlocks.Commands;

public class RequestScheduleBlockCommandHandler : IRequestHandler<RequestScheduleBlockCommand, Result<ScheduleBlockDto>>
{
    private readonly IApplicationDbContext _db;
    public RequestScheduleBlockCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ScheduleBlockDto>> Handle(RequestScheduleBlockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[RequestScheduleBlockCommandHandler] Handle chamado: {System.Text.Json.JsonSerializer.Serialize(request)}");
            if (request.StartDate > request.EndDate)
            {
                Console.WriteLine("[RequestScheduleBlockCommandHandler] Data inicial maior que a final.");
                return Result<ScheduleBlockDto>.Failure("Data inicial não pode ser maior que a final.");
            }

            var block = new ScheduleBlock
            {
                ProfessionalId = request.ProfessionalId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,
                Status = BlockStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _db.ScheduleBlocks.Add(block);
            await _db.SaveChangesAsync(cancellationToken);

            var dto = new ScheduleBlockDto
            {
                Id = block.Id,
                ProfessionalId = block.ProfessionalId,
                StartDate = block.StartDate,
                EndDate = block.EndDate,
                Reason = block.Reason,
                Status = block.Status,
                CreatedAt = block.CreatedAt
            };
            Console.WriteLine("[RequestScheduleBlockCommandHandler] Bloqueio criado com sucesso.");
            return Result<ScheduleBlockDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RequestScheduleBlockCommandHandler] Erro inesperado: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }
}
