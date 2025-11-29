using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Schedules.Commands;

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result<bool>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<ScheduleDay> _scheduleDayRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<ScheduleDay> scheduleDayRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _scheduleDayRepository = scheduleDayRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetQueryable()
            .Include(s => s.ScheduleDays)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (schedule == null)
        {
            return Result<bool>.Failure("Agenda não encontrada.");
        }

        // Remover dias
        foreach (var day in schedule.ScheduleDays)
        {
            await _scheduleDayRepository.DeleteAsync(day.Id);
        }

        await _scheduleRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
