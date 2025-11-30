using MediatR;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetProfessionalAppointmentsQuery : IRequest<Result<List<AppointmentDto>>>
{
    public int ProfessionalId { get; set; }
    public bool? IncludePast { get; set; } = false;
}
