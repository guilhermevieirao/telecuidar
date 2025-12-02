using MediatR;
using app.Application.Specialties.DTOs;

namespace app.Application.Specialties.Queries;

public record GetSpecialtyFieldsQuery(int SpecialtyId) : IRequest<List<SpecialtyFieldDto>>;
