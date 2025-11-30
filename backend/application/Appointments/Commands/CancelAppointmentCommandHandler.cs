using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Appointments.Commands;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<bool>>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Buscar o agendamento com validação de quem está cancelando
        var query = _appointmentRepository.GetQueryable()
            .Where(a => a.Id == request.AppointmentId);

        // Validar se é paciente ou profissional
        if (request.PatientId.HasValue)
        {
            query = query.Where(a => a.PatientId == request.PatientId.Value);
        }
        else if (request.ProfessionalId.HasValue)
        {
            query = query.Where(a => a.ProfessionalId == request.ProfessionalId.Value);
        }
        else
        {
            return Result<bool>.Failure("É necessário informar o paciente ou profissional");
        }

        var appointment = await query.FirstOrDefaultAsync(cancellationToken);

        if (appointment == null)
        {
            return Result<bool>.Failure("Agendamento não encontrado ou você não tem permissão para cancelá-lo");
        }

        // Verificar se já está cancelado
        if (appointment.Status == "Cancelado")
        {
            return Result<bool>.Failure("Este agendamento já foi cancelado");
        }

        // Verificar se já foi concluído
        if (appointment.Status == "Concluído")
        {
            return Result<bool>.Failure("Não é possível cancelar um agendamento já concluído");
        }

        // Cancelar o agendamento
        appointment.Status = "Cancelado";
        appointment.CancellationReason = request.CancellationReason;
        appointment.CancelledAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _appointmentRepository.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
