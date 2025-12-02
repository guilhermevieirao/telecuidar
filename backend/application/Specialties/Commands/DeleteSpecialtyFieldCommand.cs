using MediatR;

namespace app.Application.Specialties.Commands;

public record DeleteSpecialtyFieldCommand(int FieldId) : IRequest<bool>;
