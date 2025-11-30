using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using System.Globalization;

namespace app.Application.Appointments.Queries;

public class GetAvailableDatesQueryHandler : IRequestHandler<GetAvailableDatesQuery, Result<List<AvailableDateDto>>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IRepository<Appointment> _appointmentRepository;

    public GetAvailableDatesQueryHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IRepository<Appointment> appointmentRepository)
    {
        _scheduleRepository = scheduleRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<List<AvailableDateDto>>> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startDate = now.Date;
        var endDate = startDate.AddDays(request.DaysAhead);

        // Buscar profissionais da especialidade com agendas ativas
        var professionalsWithSchedules = await _userSpecialtyRepository.GetQueryable()
            .Where(us => us.SpecialtyId == request.SpecialtyId && us.IsActive)
            .Join(
                _scheduleRepository.GetQueryable()
                    .Include(s => s.ScheduleDays)
                    .Where(s => s.IsActive &&
                           s.StartDate <= endDate &&
                           (s.EndDate == null || s.EndDate >= startDate)),
                us => us.UserId,
                s => s.ProfessionalId,
                (us, s) => s
            )
            .ToListAsync(cancellationToken);

        if (!professionalsWithSchedules.Any())
        {
            return Result<List<AvailableDateDto>>.Success(new List<AvailableDateDto>());
        }

        // Buscar compromissos já agendados
        var professionalIds = professionalsWithSchedules.Select(s => s.ProfessionalId).ToList();
        var existingAppointments = await _appointmentRepository.GetQueryable()
            .Where(a => a.AppointmentDate >= startDate &&
                       a.AppointmentDate <= endDate &&
                       a.Status != "Cancelado" &&
                       a.ProfessionalId.HasValue &&
                       professionalIds.Contains(a.ProfessionalId.Value))
            .Select(a => new { a.ProfessionalId, a.AppointmentDate, a.AppointmentTime, a.DurationMinutes })
            .ToListAsync(cancellationToken);

        var availableDates = new Dictionary<DateTime, int>();

        // Para cada dia no intervalo
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var totalSlots = 0;

            // Verificar cada profissional
            foreach (var schedule in professionalsWithSchedules)
            {
                var scheduleDays = schedule.ScheduleDays.Where(sd => sd.DayOfWeek == dayOfWeek).ToList();
                
                foreach (var scheduleDay in scheduleDays)
                {
                    // Calcular slots disponíveis para este dia
                    var professionalAppointments = existingAppointments
                        .Where(a => a.ProfessionalId == schedule.ProfessionalId && a.AppointmentDate.Date == date)
                        .Select(a => a.AppointmentTime)
                        .ToList();
                    
                    var slots = CalculateAvailableSlots(scheduleDay, professionalAppointments);
                    
                    totalSlots += slots;
                }
            }

            if (totalSlots > 0)
            {
                availableDates[date] = totalSlots;
            }
        }

        var result = availableDates.Select(kvp => new AvailableDateDto
        {
            Date = kvp.Key,
            DateFormatted = kvp.Key.ToString("dd/MM/yyyy"),
            DayOfWeek = GetDayOfWeekName(kvp.Key.DayOfWeek),
            AvailableSlotsCount = kvp.Value
        }).OrderBy(d => d.Date).ToList();

        return Result<List<AvailableDateDto>>.Success(result);
    }

    private int CalculateAvailableSlots(ScheduleDay scheduleDay, List<TimeSpan> bookedTimes)
    {
        var slots = new List<TimeSpan>();
        var currentTime = scheduleDay.StartTime;
        var totalMinutes = scheduleDay.AppointmentDuration + scheduleDay.IntervalBetweenAppointments;

        while (currentTime.Add(TimeSpan.FromMinutes(scheduleDay.AppointmentDuration)) <= scheduleDay.EndTime)
        {
            // Verificar se está no horário de pausa
            if (scheduleDay.BreakStartTime.HasValue && scheduleDay.BreakEndTime.HasValue)
            {
                if (currentTime >= scheduleDay.BreakStartTime.Value && currentTime < scheduleDay.BreakEndTime.Value)
                {
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(totalMinutes));
                    continue;
                }
            }

            // Verificar se já tem compromisso neste horário
            var isBooked = bookedTimes.Contains(currentTime);
            
            if (!isBooked)
            {
                slots.Add(currentTime);
            }

            currentTime = currentTime.Add(TimeSpan.FromMinutes(totalMinutes));
        }

        return slots.Count;
    }

    private static string GetDayOfWeekName(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terça-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            DayOfWeek.Saturday => "Sábado",
            _ => ""
        };
    }
}
