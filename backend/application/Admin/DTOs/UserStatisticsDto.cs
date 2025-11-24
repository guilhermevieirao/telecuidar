using app.Domain.Enums;

namespace app.Application.Admin.DTOs;

public class UserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalPacientes { get; set; }
    public int TotalProfissionais { get; set; }
    public int TotalAdministradores { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int UsersToday { get; set; }
    public int UsersThisWeek { get; set; }
    public int UsersThisMonth { get; set; }
}
