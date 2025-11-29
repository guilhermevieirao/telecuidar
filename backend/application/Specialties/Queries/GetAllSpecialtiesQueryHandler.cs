using MediatR;
using app.Application.Common.Models;
using app.Application.Specialties.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Queries;

public class GetAllSpecialtiesQueryHandler : IRequestHandler<GetAllSpecialtiesQuery, Result<List<SpecialtyDto>>>
{
    private readonly IRepository<Specialty> _specialtyRepository;

    public GetAllSpecialtiesQueryHandler(IRepository<Specialty> specialtyRepository)
    {
        _specialtyRepository = specialtyRepository;
    }

    public async Task<Result<List<SpecialtyDto>>> Handle(GetAllSpecialtiesQuery request, CancellationToken cancellationToken)
    {
        var query = _specialtyRepository.GetQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        var specialties = await query
            .Include(s => s.UserSpecialties)
            .OrderBy(s => s.Name)
            .Select(s => new SpecialtyDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Icon = s.Icon,
                ProfessionalsCount = s.UserSpecialties.Count,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<SpecialtyDto>>.Success(specialties);
    }
}
