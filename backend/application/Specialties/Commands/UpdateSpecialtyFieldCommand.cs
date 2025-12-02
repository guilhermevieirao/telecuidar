using MediatR;
using app.Application.Specialties.DTOs;

namespace app.Application.Specialties.Commands;

public record UpdateSpecialtyFieldCommand(int FieldId, UpdateSpecialtyFieldDto FieldDto) : IRequest<SpecialtyFieldDto>;
