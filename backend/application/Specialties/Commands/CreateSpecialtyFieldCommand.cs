using MediatR;
using app.Application.Specialties.DTOs;

namespace app.Application.Specialties.Commands;

public record CreateSpecialtyFieldCommand(int SpecialtyId, CreateSpecialtyFieldDto FieldDto) : IRequest<SpecialtyFieldDto>;
