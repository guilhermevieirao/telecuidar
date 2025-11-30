using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Appointments.Commands;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppointmentCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IRepository<Schedule> scheduleRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Validar se o paciente existe
        var patient = await _userRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
        {
            return Result<int>.Failure("Paciente não encontrado");
        }

        // Se um profissional específico foi selecionado, validar
        if (request.ProfessionalId.HasValue)
        {
            // Validar se o profissional existe
            var professional = await _userRepository.GetByIdAsync(request.ProfessionalId.Value);
            if (professional == null)
            {
                return Result<int>.Failure("Profissional não encontrado");
            }

            // Validar se o profissional tem a especialidade
            var hasSpecialty = await _userSpecialtyRepository.GetQueryable()
                .AnyAsync(us => us.UserId == request.ProfessionalId.Value && 
                               us.SpecialtyId == request.SpecialtyId && 
                               us.IsActive, 
                          cancellationToken);

            if (!hasSpecialty)
            {
                return Result<int>.Failure("Profissional não possui esta especialidade");
            }
        }

        var targetDate = request.AppointmentDate.Date;
        var dayOfWeek = (int)targetDate.DayOfWeek;

        Schedule? schedule = null;
        ScheduleDay? scheduleDay = null;

        // Se um profissional específico foi selecionado, verificar sua agenda
        if (request.ProfessionalId.HasValue)
        {
            // Verificar se o profissional tem agenda ativa para este dia
            schedule = await _scheduleRepository.GetQueryable()
                .Include(s => s.ScheduleDays)
                .Where(s => s.ProfessionalId == request.ProfessionalId.Value &&
                           s.IsActive &&
                           s.StartDate <= targetDate &&
                           (s.EndDate == null || s.EndDate >= targetDate) &&
                           s.ScheduleDays.Any(sd => sd.DayOfWeek == dayOfWeek))
                .FirstOrDefaultAsync(cancellationToken);

            if (schedule == null)
            {
                return Result<int>.Failure("Profissional não possui agenda disponível para este dia");
            }

            scheduleDay = schedule.ScheduleDays.FirstOrDefault(sd => sd.DayOfWeek == dayOfWeek);
        }
        else
        {
            // Sem preferência - buscar qualquer profissional disponível
            var schedules = await _scheduleRepository.GetQueryable()
                .Include(s => s.ScheduleDays)
                .Where(s => s.IsActive &&
                           s.StartDate <= targetDate &&
                           (s.EndDate == null || s.EndDate >= targetDate) &&
                           s.ScheduleDays.Any(sd => sd.DayOfWeek == dayOfWeek))
                .ToListAsync(cancellationToken);

            // Buscar primeiro profissional com horário livre
            foreach (var s in schedules)
            {
                var sd = s.ScheduleDays.FirstOrDefault(sd => sd.DayOfWeek == dayOfWeek);
                if (sd != null)
                {
                    var hasAppointment = await _appointmentRepository.GetQueryable()
                        .AnyAsync(a => a.ProfessionalId == s.ProfessionalId &&
                                      a.AppointmentDate.Date == targetDate &&
                                      a.AppointmentTime == request.AppointmentTime &&
                                      a.Status != "Cancelado",
                                 cancellationToken);

                    if (!hasAppointment)
                    {
                        request.ProfessionalId = s.ProfessionalId;
                        schedule = s;
                        scheduleDay = sd;
                        break;
                    }
                }
            }

            if (schedule == null || !request.ProfessionalId.HasValue)
            {
                return Result<int>.Failure("Não há profissionais disponíveis para este horário");
            }
        }

        // Validar se o horário está dentro do range
        if (scheduleDay != null && (request.AppointmentTime < scheduleDay.StartTime || 
            request.AppointmentTime.Add(TimeSpan.FromMinutes(scheduleDay.AppointmentDuration)) > scheduleDay.EndTime))
        {
            return Result<int>.Failure("Horário fora do expediente do profissional");
        }

        // Verificar se está no horário de pausa
        if (scheduleDay != null && scheduleDay.BreakStartTime.HasValue && scheduleDay.BreakEndTime.HasValue)
        {
            if (request.AppointmentTime >= scheduleDay.BreakStartTime.Value && 
                request.AppointmentTime < scheduleDay.BreakEndTime.Value)
            {
                return Result<int>.Failure("Horário está no período de pausa do profissional");
            }
        }

        // Verificar se já existe um agendamento neste horário para o profissional
        var existingAppointment = await _appointmentRepository.GetQueryable()
            .AnyAsync(a => a.ProfessionalId == request.ProfessionalId.Value &&
                          a.AppointmentDate.Date == targetDate &&
                          a.AppointmentTime == request.AppointmentTime &&
                          a.Status != "Cancelado",
                     cancellationToken);

        if (existingAppointment)
        {
            return Result<int>.Failure("Este horário já está ocupado");
        }

        // Criar o agendamento
        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            ProfessionalId = request.ProfessionalId,
            SpecialtyId = request.SpecialtyId,
            AppointmentDate = targetDate,
            AppointmentTime = request.AppointmentTime,
            DurationMinutes = scheduleDay.AppointmentDuration,
            Status = "Agendado",
            Notes = request.Notes,
            MeetingRoomId = GenerateMeetingRoomId(request.PatientId, request.ProfessionalId.Value, targetDate, request.AppointmentTime),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _appointmentRepository.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(appointment.Id);
    }

    private string GenerateMeetingRoomId(int patientId, int professionalId, DateTime date, TimeSpan time)
    {
        // Gerar um ID único e previsível para a sala de reunião
        var dateStr = date.ToString("yyyyMMdd");
        var timeStr = time.ToString(@"hhmm");
        var uniqueId = $"TC-{patientId}-{professionalId}-{dateStr}-{timeStr}";
        return uniqueId;
    }
}
