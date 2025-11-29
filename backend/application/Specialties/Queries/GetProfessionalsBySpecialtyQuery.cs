using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;

namespace app.Application.Specialties.Queries;

public class GetProfessionalsBySpecialtyQuery : IRequest<Result<List<UserDto>>>
{
    public int SpecialtyId { get; set; }
}
