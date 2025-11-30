using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Appointments.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Appointments.Queries;

public class GetAvailableSpecialtiesQueryHandler : IRequestHandler<GetAvailableSpecialtiesQuery, Result<List<AvailableSpecialtyDto>>>
{
    private readonly IRepository<Specialty> _specialtyRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IRepository<Schedule> _scheduleRepository;

    public GetAvailableSpecialtiesQueryHandler(
        IRepository<Specialty> specialtyRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IRepository<Schedule> scheduleRepository)
    {
        _specialtyRepository = specialtyRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<List<AvailableSpecialtyDto>>> Handle(GetAvailableSpecialtiesQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        
        // Buscar especialidades que possuem profissionais com agendas ativas
        var availableSpecialties = await _specialtyRepository.GetQueryable()
            .Where(s => s.IsActive)
            .Select(s => new
            {
                Specialty = s,
                ProfessionalsCount = _userSpecialtyRepository.GetQueryable()
                    .Where(us => us.SpecialtyId == s.Id && us.IsActive)
                    .Join(
                        _scheduleRepository.GetQueryable()
                            .Where(sch => sch.IsActive && 
                                   sch.StartDate <= now &&
                                   (sch.EndDate == null || sch.EndDate >= now)),
                        us => us.UserId,
                        sch => sch.ProfessionalId,
                        (us, sch) => us.UserId
                    )
                    .Distinct()
                    .Count()
            })
            .Where(x => x.ProfessionalsCount > 0)
            .OrderBy(x => x.Specialty.Name)
            .ToListAsync(cancellationToken);

        var result = availableSpecialties.Select(x => new AvailableSpecialtyDto
        {
            Id = x.Specialty.Id,
            Name = x.Specialty.Name,
            Description = x.Specialty.Description,
            Icon = x.Specialty.Icon,
            AvailableProfessionalsCount = x.ProfessionalsCount
        }).ToList();

        return Result<List<AvailableSpecialtyDto>>.Success(result);
    }
}
