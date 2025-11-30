using MediatR;
using app.Application.Common.Models;

namespace app.Application.Appointments.Commands;

public class CancelAppointmentCommand : IRequest<Result<bool>>
{
    public int AppointmentId { get; set; }
    public int? PatientId { get; set; }
    public int? ProfessionalId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}
