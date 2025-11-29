using MediatR;
using app.Application.Common.Models;
using app.Application.Specialties.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Queries;

public class GetUserSpecialtiesQueryHandler : IRequestHandler<GetUserSpecialtiesQuery, Result<List<UserSpecialtyDto>>>
{
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;

    public GetUserSpecialtiesQueryHandler(IRepository<UserSpecialty> userSpecialtyRepository)
    {
        _userSpecialtyRepository = userSpecialtyRepository;
    }

    public async Task<Result<List<UserSpecialtyDto>>> Handle(GetUserSpecialtiesQuery request, CancellationToken cancellationToken)
    {
        var userSpecialties = await _userSpecialtyRepository.GetQueryable()
            .Where(us => us.UserId == request.UserId)
            .Include(us => us.Specialty)
            .Select(us => new UserSpecialtyDto
            {
                Id = us.Id,
                UserId = us.UserId,
                SpecialtyId = us.SpecialtyId,
                SpecialtyName = us.Specialty.Name,
                Icon = us.Specialty.Icon,
                Description = us.Specialty.Description,
                AssignedAt = us.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<UserSpecialtyDto>>.Success(userSpecialties);
    }
}
