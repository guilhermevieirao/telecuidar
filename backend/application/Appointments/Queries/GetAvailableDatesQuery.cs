using MediatR;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetAvailableDatesQuery : IRequest<Result<List<AvailableDateDto>>>
{
    public int SpecialtyId { get; set; }
    public int DaysAhead { get; set; } = 30; // Padrão: próximos 30 dias
}
