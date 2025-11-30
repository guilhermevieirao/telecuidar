using MediatR;
using app.Application.Common.Models;
using app.Application.Schedules.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace app.Application.Schedules.Queries;

public class GetAllSchedulesQueryHandler : IRequestHandler<GetAllSchedulesQuery, Result<List<ScheduleDto>>>
{
    private readonly IRepository<Schedule> _scheduleRepository;

    public GetAllSchedulesQueryHandler(IRepository<Schedule> scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<List<ScheduleDto>>> Handle(GetAllSchedulesQuery request, CancellationToken cancellationToken)
    {
        var query = _scheduleRepository.GetQueryable()
            .Include(s => s.Professional)
            .Include(s => s.CreatedByUser)
            .Include(s => s.ScheduleDays)
            .AsQueryable();

        if (request.ProfessionalId.HasValue)
        {
            query = query.Where(s => s.ProfessionalId == request.ProfessionalId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var schedules = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                ProfessionalId = s.ProfessionalId,
                ProfessionalName = s.Professional.FullName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedByName = s.CreatedByUser != null ? s.CreatedByUser.FullName : null,
                ScheduleDays = s.ScheduleDays.OrderBy(d => d.DayOfWeek).Select(d => new ScheduleDayDto
                {
                    Id = d.Id,
                    DayOfWeek = d.DayOfWeek,
                    DayOfWeekName = GetDayOfWeekName(d.DayOfWeek),
                    StartTime = d.StartTime.ToString(@"hh\:mm"),
                    EndTime = d.EndTime.ToString(@"hh\:mm"),
                    AppointmentDuration = d.AppointmentDuration,
                    IntervalBetweenAppointments = d.IntervalBetweenAppointments,
                    BreakStartTime = d.BreakStartTime.HasValue ? d.BreakStartTime.Value.ToString(@"hh\:mm") : null,
                    BreakEndTime = d.BreakEndTime.HasValue ? d.BreakEndTime.Value.ToString(@"hh\:mm") : null
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<List<ScheduleDto>>.Success(schedules);
    }

    private static string GetDayOfWeekName(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            0 => "Domingo",
            1 => "Segunda-feira",
            2 => "Terça-feira",
            3 => "Quarta-feira",
            4 => "Quinta-feira",
            5 => "Sexta-feira",
            6 => "Sábado",
            _ => ""
        };
    }
}
