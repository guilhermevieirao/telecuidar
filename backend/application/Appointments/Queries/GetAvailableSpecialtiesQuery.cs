using MediatR;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetAvailableSpecialtiesQuery : IRequest<Result<List<AvailableSpecialtyDto>>>
{
}
