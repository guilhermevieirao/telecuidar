using MediatR;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public record GetAppointmentFieldValuesQuery(int AppointmentId) : IRequest<List<AppointmentFieldValueDto>>;
