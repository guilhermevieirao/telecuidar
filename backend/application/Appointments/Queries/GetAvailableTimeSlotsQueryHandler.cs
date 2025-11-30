using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Appointments.Queries;

public class GetAvailableTimeSlotsQueryHandler : IRequestHandler<GetAvailableTimeSlotsQuery, Result<List<AvailableTimeSlotDto>>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<User> _userRepository;

    public GetAvailableTimeSlotsQueryHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IRepository<Appointment> appointmentRepository,
        IRepository<User> userRepository)
    {
        _scheduleRepository = scheduleRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<AvailableTimeSlotDto>>> Handle(GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date.Date;
        var dayOfWeek = (int)targetDate.DayOfWeek;

        // Buscar profissionais da especialidade com agendas ativas para o dia solicitado
        var professionalsWithSchedules = await _userSpecialtyRepository.GetQueryable()
            .Where(us => us.SpecialtyId == request.SpecialtyId && us.IsActive)
            .Join(
                _scheduleRepository.GetQueryable()
                    .Include(s => s.ScheduleDays)
                    .Include(s => s.Professional)
                    .Where(s => s.IsActive &&
                           s.StartDate <= targetDate &&
                           (s.EndDate == null || s.EndDate >= targetDate) &&
                           s.ScheduleDays.Any(sd => sd.DayOfWeek == dayOfWeek)),
                us => us.UserId,
                s => s.ProfessionalId,
                (us, s) => s
            )
            .ToListAsync(cancellationToken);

        if (!professionalsWithSchedules.Any())
        {
            return Result<List<AvailableTimeSlotDto>>.Success(new List<AvailableTimeSlotDto>());
        }

        // Buscar compromissos já agendados para este dia
        var professionalIds = professionalsWithSchedules.Select(s => s.ProfessionalId).ToList();
        var existingAppointments = await _appointmentRepository.GetQueryable()
            .Where(a => a.AppointmentDate.Date == targetDate &&
                       a.Status != "Cancelado" &&
                       a.ProfessionalId.HasValue &&
                       professionalIds.Contains(a.ProfessionalId.Value))
            .Select(a => new { a.ProfessionalId, a.AppointmentTime })
            .ToListAsync(cancellationToken);

        // Dicionário: Horário -> Lista de Profissionais disponíveis
        var timeSlots = new Dictionary<TimeSpan, List<AvailableProfessionalDto>>();

        foreach (var schedule in professionalsWithSchedules)
        {
            var scheduleDays = schedule.ScheduleDays.Where(sd => sd.DayOfWeek == dayOfWeek).ToList();
            
            foreach (var scheduleDay in scheduleDays)
            {
                var availableSlots = CalculateAvailableSlots(
                    scheduleDay,
                    existingAppointments
                        .Where(a => a.ProfessionalId == schedule.ProfessionalId)
                        .Select(a => a.AppointmentTime)
                        .ToList()
                );

                var professionalDto = new AvailableProfessionalDto
                {
                    Id = schedule.Professional.Id,
                    Name = schedule.Professional.FullName,
                    ProfilePhotoUrl = schedule.Professional.ProfilePhotoUrl
                };

                foreach (var slot in availableSlots)
                {
                    if (!timeSlots.ContainsKey(slot))
                    {
                        timeSlots[slot] = new List<AvailableProfessionalDto>();
                    }
                    
                    // Adicionar profissional apenas se ainda não estiver na lista deste horário
                    if (!timeSlots[slot].Any(p => p.Id == professionalDto.Id))
                    {
                        timeSlots[slot].Add(professionalDto);
                    }
                }
            }
        }

        var result = timeSlots
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new AvailableTimeSlotDto
            {
                Time = kvp.Key.ToString(@"hh\:mm"),
                Professionals = kvp.Value.OrderBy(p => p.Name).ToList()
            })
            .ToList();

        return Result<List<AvailableTimeSlotDto>>.Success(result);
    }

    private List<TimeSpan> CalculateAvailableSlots(ScheduleDay scheduleDay, List<TimeSpan> bookedTimes)
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
            if (!bookedTimes.Contains(currentTime))
            {
                slots.Add(currentTime);
            }

            currentTime = currentTime.Add(TimeSpan.FromMinutes(totalMinutes));
        }

        return slots;
    }
}
