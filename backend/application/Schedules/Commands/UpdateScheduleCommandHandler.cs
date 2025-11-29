using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Schedules.Commands;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<bool>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<ScheduleDay> _scheduleDayRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<ScheduleDay> scheduleDayRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _scheduleDayRepository = scheduleDayRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetQueryable()
            .Include(s => s.ScheduleDays)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (schedule == null)
        {
            return Result<bool>.Failure("Agenda não encontrada.");
        }

        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
        {
            return Result<bool>.Failure("A data de término deve ser posterior à data de início.");
        }

        schedule.StartDate = request.StartDate;
        schedule.EndDate = request.EndDate;
        schedule.IsActive = request.IsActive;
        schedule.UpdatedAt = DateTime.UtcNow;

        // Remover dias antigos
        foreach (var oldDay in schedule.ScheduleDays.ToList())
        {
            await _scheduleDayRepository.DeleteAsync(oldDay.Id);
        }

        // Adicionar novos dias
        foreach (var dayDto in request.ScheduleDays)
        {
            if (!TimeSpan.TryParse(dayDto.StartTime, out var startTime) ||
                !TimeSpan.TryParse(dayDto.EndTime, out var endTime))
            {
                return Result<bool>.Failure("Horários inválidos.");
            }

            TimeSpan? breakStart = null;
            TimeSpan? breakEnd = null;

            if (!string.IsNullOrEmpty(dayDto.BreakStartTime) && !string.IsNullOrEmpty(dayDto.BreakEndTime))
            {
                if (!TimeSpan.TryParse(dayDto.BreakStartTime, out var bStart) ||
                    !TimeSpan.TryParse(dayDto.BreakEndTime, out var bEnd))
                {
                    return Result<bool>.Failure("Horários de pausa inválidos.");
                }

                breakStart = bStart;
                breakEnd = bEnd;
            }

            var scheduleDay = new ScheduleDay
            {
                ScheduleId = schedule.Id,
                DayOfWeek = dayDto.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                AppointmentDuration = dayDto.AppointmentDuration,
                IntervalBetweenAppointments = dayDto.IntervalBetweenAppointments,
                BreakStartTime = breakStart,
                BreakEndTime = breakEnd,
                CreatedAt = DateTime.UtcNow
            };

            await _scheduleDayRepository.AddAsync(scheduleDay);
        }

        await _scheduleRepository.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
