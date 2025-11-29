using MediatR;
using app.Application.Common.Models;

namespace app.Application.Specialties.Commands;

public class RemoveSpecialtyFromProfessionalCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public int SpecialtyId { get; set; }
}
