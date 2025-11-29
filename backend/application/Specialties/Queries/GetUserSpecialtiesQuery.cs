using MediatR;
using app.Application.Common.Models;
using app.Application.Specialties.DTOs;

namespace app.Application.Specialties.Queries;

public class GetUserSpecialtiesQuery : IRequest<Result<List<UserSpecialtyDto>>>
{
    public int UserId { get; set; }
}
