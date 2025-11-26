using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Reports.DTOs;
using app.Domain.Interfaces;
using app.Domain.Entities;

namespace app.Application.Reports.Queries.GetUsersReport;

public class GetUsersReportQueryHandler : IRequestHandler<GetUsersReportQuery, UsersReportDto>
{
    private readonly IRepository<User> _userRepository;

    public GetUsersReportQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UsersReportDto> Handle(GetUsersReportQuery request, CancellationToken cancellationToken)
    {
        var query = _userRepository.GetQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(u => u.CreatedAt >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(u => u.CreatedAt <= request.EndDate.Value);

        var users = await query.ToListAsync(cancellationToken);

        var report = new UsersReportDto
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            InactiveUsers = users.Count(u => !u.IsActive),
            EmailConfirmedCount = users.Count(u => u.EmailConfirmed),
            PacientesCount = users.Count(u => u.Role == Domain.Enums.UserRole.Paciente),
            ProfissionaisCount = users.Count(u => u.Role == Domain.Enums.UserRole.Profissional),
            AdministradoresCount = users.Count(u => u.Role == Domain.Enums.UserRole.Administrador),
            GeneratedAt = DateTime.Now,
            UserDetails = users.Select(u => new UserDetailDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = GetRoleName(u.Role),
                IsActive = u.IsActive,
                EmailConfirmed = u.EmailConfirmed,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList()
        };

        return report;
    }

    private string GetRoleName(Domain.Enums.UserRole role)
    {
        return role switch
        {
            Domain.Enums.UserRole.Paciente => "Paciente",
            Domain.Enums.UserRole.Profissional => "Profissional",
            Domain.Enums.UserRole.Administrador => "Administrador",
            _ => "Desconhecido"
        };
    }
}
