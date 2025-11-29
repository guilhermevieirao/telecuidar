using MediatR;
using app.Application.Common.Models;
using app.Application.Users.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Queries;

public class GetProfessionalsBySpecialtyQueryHandler : IRequestHandler<GetProfessionalsBySpecialtyQuery, Result<List<UserDto>>>
{
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;

    public GetProfessionalsBySpecialtyQueryHandler(IRepository<UserSpecialty> userSpecialtyRepository)
    {
        _userSpecialtyRepository = userSpecialtyRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetProfessionalsBySpecialtyQuery request, CancellationToken cancellationToken)
    {
        var professionals = await _userSpecialtyRepository.GetQueryable()
            .Where(us => us.SpecialtyId == request.SpecialtyId)
            .Include(us => us.User)
            .Select(us => new UserDto
            {
                Id = us.User.Id,
                FirstName = us.User.FirstName,
                LastName = us.User.LastName,
                Email = us.User.Email,
                FullName = us.User.FullName,
                PhoneNumber = us.User.PhoneNumber,
                Role = us.User.Role,
                ProfilePhotoUrl = us.User.ProfilePhotoUrl,
                EmailConfirmed = us.User.EmailConfirmed,
                CreatedAt = us.User.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<UserDto>>.Success(professionals);
    }
}
