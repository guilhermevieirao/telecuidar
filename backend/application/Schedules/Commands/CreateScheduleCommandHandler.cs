using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Schedules.Commands;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<int>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        // Validar profissional
        var professional = await _userRepository.GetByIdAsync(request.ProfessionalId);
        if (professional == null)
        {
            return Result<int>.Failure("Profissional não encontrado.");
        }

        if (professional.Role != UserRole.Profissional)
        {
            return Result<int>.Failure("O usuário deve ser um profissional.");
        }

        // Validar datas
        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
        {
            return Result<int>.Failure("A data de término deve ser posterior à data de início.");
        }

        // Validar dias
        if (request.ScheduleDays == null || request.ScheduleDays.Count == 0)
        {
            return Result<int>.Failure("É necessário definir pelo menos um dia de atendimento.");
        }

        var schedule = new Schedule
        {
            ProfessionalId = request.ProfessionalId,
            CreatedByUserId = request.CreatedByUserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var dayDto in request.ScheduleDays)
        {
            if (!TimeSpan.TryParse(dayDto.StartTime, out var startTime) ||
                !TimeSpan.TryParse(dayDto.EndTime, out var endTime))
            {
                return Result<int>.Failure("Horários inválidos.");
            }

            if (endTime <= startTime)
            {
                return Result<int>.Failure("O horário de término deve ser posterior ao horário de início.");
            }

            TimeSpan? breakStart = null;
            TimeSpan? breakEnd = null;

            if (!string.IsNullOrEmpty(dayDto.BreakStartTime) && !string.IsNullOrEmpty(dayDto.BreakEndTime))
            {
                if (!TimeSpan.TryParse(dayDto.BreakStartTime, out var bStart) ||
                    !TimeSpan.TryParse(dayDto.BreakEndTime, out var bEnd))
                {
                    return Result<int>.Failure("Horários de pausa inválidos.");
                }

                if (bEnd <= bStart)
                {
                    return Result<int>.Failure("O horário de término da pausa deve ser posterior ao horário de início.");
                }

                if (bStart < startTime || bEnd > endTime)
                {
                    return Result<int>.Failure("A pausa deve estar dentro do horário de atendimento.");
                }

                breakStart = bStart;
                breakEnd = bEnd;
            }

            var scheduleDay = new ScheduleDay
            {
                DayOfWeek = dayDto.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                AppointmentDuration = dayDto.AppointmentDuration,
                IntervalBetweenAppointments = dayDto.IntervalBetweenAppointments,
                BreakStartTime = breakStart,
                BreakEndTime = breakEnd,
                CreatedAt = DateTime.UtcNow
            };

            schedule.ScheduleDays.Add(scheduleDay);
        }

        await _scheduleRepository.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(schedule.Id);
    }
}
