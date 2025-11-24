using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;
using app.Domain.Entities;
using app.Domain.Enums;
using app.Domain.Interfaces;

namespace app.Application.Admin.Queries.GetUserStatistics;

public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, Result<UserStatisticsDto>>
{
    private readonly IRepository<User> _userRepository;

    public GetUserStatisticsQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            var statistics = new UserStatisticsDto
            {
                TotalUsers = users.Count(),
                TotalPacientes = users.Count(u => u.Role == UserRole.Paciente),
                TotalProfissionais = users.Count(u => u.Role == UserRole.Profissional),
                TotalAdministradores = users.Count(u => u.Role == UserRole.Administrador),
                ActiveUsers = users.Count(u => u.IsActive),
                InactiveUsers = users.Count(u => !u.IsActive),
                UsersToday = users.Count(u => u.CreatedAt.Date == now.Date),
                UsersThisWeek = users.Count(u => u.CreatedAt >= now.AddDays(-7)),
                UsersThisMonth = users.Count(u => u.CreatedAt >= now.AddDays(-30))
            };

            return Result<UserStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<UserStatisticsDto>.Failure($"Erro ao obter estatísticas: {ex.Message}");
        }
    }
}
