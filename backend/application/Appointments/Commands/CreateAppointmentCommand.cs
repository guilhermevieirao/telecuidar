using MediatR;
using app.Application.Common.Models;

namespace app.Application.Appointments.Commands;

public class CreateAppointmentCommand : IRequest<Result<int>>
{
    public int PatientId { get; set; }
    public int? ProfessionalId { get; set; }
    public int SpecialtyId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string? Notes { get; set; }
}
