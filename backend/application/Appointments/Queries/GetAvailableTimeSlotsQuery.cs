using MediatR;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetAvailableTimeSlotsQuery : IRequest<Result<List<AvailableTimeSlotDto>>>
{
    public int SpecialtyId { get; set; }
    public DateTime Date { get; set; }
}
