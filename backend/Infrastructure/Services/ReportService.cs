using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid? userId = null)
    {
        var appointmentsQuery = _context.Appointments.AsQueryable();
        var notificationsQuery = _context.Notifications.AsQueryable();

        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                if (user.Role == UserRole.PATIENT)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.PatientId == userId.Value);
                }
                else if (user.Role == UserRole.PROFESSIONAL)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.ProfessionalId == userId.Value);
                }

                notificationsQuery = notificationsQuery.Where(n => n.UserId == userId.Value);
            }
        }

        var appointmentStats = new AppointmentStatsDto
        {
            Total = await appointmentsQuery.CountAsync(),
            Scheduled = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Scheduled),
            Confirmed = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Confirmed),
            InProgress = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.InProgress),
            Completed = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Completed),
            Cancelled = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Cancelled)
        };

        var userStats = new UserStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            Patients = await _context.Users.CountAsync(u => u.Role == UserRole.PATIENT),
            Professionals = await _context.Users.CountAsync(u => u.Role == UserRole.PROFESSIONAL),
            Admins = await _context.Users.CountAsync(u => u.Role == UserRole.ADMIN),
            ActiveUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Active),
            InactiveUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Inactive)
        };

        var topSpecialties = await _context.Specialties
            .Select(s => new SpecialtyStatsDto
            {
                SpecialtyId = s.Id,
                SpecialtyName = s.Name,
                AppointmentCount = s.Appointments.Count,
                ProfessionalCount = s.Professionals.Count
            })
            .OrderByDescending(s => s.AppointmentCount)
            .Take(5)
            .ToListAsync();

        var notificationStats = await notificationsQuery
            .GroupBy(n => n.IsRead)
            .Select(g => new { IsRead = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalNotifications = notificationStats.Sum(n => n.Count);
        var unreadNotifications = notificationStats.FirstOrDefault(n => !n.IsRead)?.Count ?? 0;

        return new DashboardStatsDto
        {
            Appointments = appointmentStats,
            Users = userStats,
            TopSpecialties = topSpecialties,
            TotalNotifications = totalNotifications,
            UnreadNotifications = unreadNotifications
        };
    }

    public async Task<ReportDto> GenerateAppointmentsReportAsync(DateTime startDate, DateTime endDate)
    {
        var appointments = await _context.Appointments
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .Include(a => a.Specialty)
            .ToListAsync();

        var appointmentStats = new AppointmentStatsDto
        {
            Total = appointments.Count,
            Scheduled = appointments.Count(a => a.Status == AppointmentStatus.Scheduled),
            Confirmed = appointments.Count(a => a.Status == AppointmentStatus.Confirmed),
            InProgress = appointments.Count(a => a.Status == AppointmentStatus.InProgress),
            Completed = appointments.Count(a => a.Status == AppointmentStatus.Completed),
            Cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled)
        };

        var appointmentsByDay = appointments
            .GroupBy(a => a.Date.Date)
            .Select(g => new AppointmentsByPeriodDto
            {
                Period = g.Key.ToString("yyyy-MM-dd"),
                Count = g.Count()
            })
            .OrderBy(x => x.Period)
            .ToList();

        var specialtiesReport = appointments
            .GroupBy(a => new { a.SpecialtyId, a.Specialty.Name })
            .Select(g => new SpecialtyStatsDto
            {
                SpecialtyId = g.Key.SpecialtyId,
                SpecialtyName = g.Key.Name,
                AppointmentCount = g.Count(),
                ProfessionalCount = g.Select(a => a.ProfessionalId).Distinct().Count()
            })
            .OrderByDescending(s => s.AppointmentCount)
            .ToList();

        return new ReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            StartDate = startDate,
            EndDate = endDate,
            Appointments = appointmentStats,
            AppointmentsByDay = appointmentsByDay,
            SpecialtiesReport = specialtiesReport
        };
    }

    public async Task<List<SpecialtyStatsDto>> GetSpecialtyStatsAsync()
    {
        return await _context.Specialties
            .Select(s => new SpecialtyStatsDto
            {
                SpecialtyId = s.Id,
                SpecialtyName = s.Name,
                AppointmentCount = s.Appointments.Count,
                ProfessionalCount = s.Professionals.Count
            })
            .OrderByDescending(s => s.AppointmentCount)
            .ToListAsync();
    }
}
