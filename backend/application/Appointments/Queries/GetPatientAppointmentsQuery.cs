using MediatR;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetPatientAppointmentsQuery : IRequest<Result<List<AppointmentDto>>>
{
    public int PatientId { get; set; }
    public bool? IncludePast { get; set; } = false;
}
