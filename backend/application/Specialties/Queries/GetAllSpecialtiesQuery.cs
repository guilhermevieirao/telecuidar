using MediatR;
using app.Application.Common.Models;
using app.Application.Specialties.DTOs;

namespace app.Application.Specialties.Queries;

public class GetAllSpecialtiesQuery : IRequest<Result<List<SpecialtyDto>>>
{
    public bool IncludeInactive { get; set; } = false;
}
