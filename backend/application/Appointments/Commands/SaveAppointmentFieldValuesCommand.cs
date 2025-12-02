using MediatR;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Commands;

public record SaveAppointmentFieldValuesCommand(BulkSaveAppointmentFieldValuesDto Dto) : IRequest<List<AppointmentFieldValueDto>>;
