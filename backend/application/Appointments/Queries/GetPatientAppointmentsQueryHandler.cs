using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Appointments.Queries;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, Result<List<AppointmentDto>>>
{
    private readonly IRepository<Appointment> _appointmentRepository;

    public GetPatientAppointmentsQueryHandler(IRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _appointmentRepository.GetQueryable()
                .Include(a => a.Patient)
                .Include(a => a.Professional)
                .Include(a => a.Specialty)
                .Where(a => a.PatientId == request.PatientId && a.IsActive);

            if (!request.IncludePast.GetValueOrDefault())
            {
                var now = DateTime.UtcNow.Date;
                query = query.Where(a => a.AppointmentDate >= now || a.Status == "Agendado" || a.Status == "Confirmado");
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync(cancellationToken);

            // Ordenar por tempo em memória
            var orderedAppointments = appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();

            var appointmentDtos = orderedAppointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FullName ?? "Desconhecido",
                ProfessionalId = a.ProfessionalId,
                ProfessionalName = a.Professional?.FullName ?? "Qualquer profissional",
                SpecialtyId = a.SpecialtyId,
                SpecialtyName = a.Specialty?.Name ?? "Não especificada",
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime.ToString(@"hh\:mm"),
                DurationMinutes = a.DurationMinutes,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Result<List<AppointmentDto>>.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AppointmentDto>>.Failure($"Erro ao buscar consultas: {ex.Message}");
        }
    }
}
