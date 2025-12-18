using Application.DTOs.Schedules;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly ApplicationDbContext _context;

    public ScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedSchedulesDto> GetSchedulesAsync(int page, int pageSize, string? search, string? status)
    {
        var query = _context.Schedules
            .Include(s => s.Professional)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
        {
            var isActive = status.ToLower() == "active";
            query = query.Where(s => s.IsActive == isActive);
        }

        // Filter by search (professional name)
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => 
                (s.Professional.Name + " " + s.Professional.LastName).ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var schedules = await query
            .OrderBy(s => s.Professional.Name)
            .ThenBy(s => s.DayOfWeek)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                ProfessionalId = s.ProfessionalId,
                ProfessionalName = s.Professional.Name + " " + s.Professional.LastName,
                DayOfWeek = s.DayOfWeek,
                DayOfWeekName = GetDayOfWeekName(s.DayOfWeek),
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                SlotDurationMinutes = s.SlotDurationMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return new PaginatedSchedulesDto
        {
            Data = schedules,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<List<ScheduleDto>> GetSchedulesByProfessionalAsync(Guid professionalId)
    {
        var schedules = await _context.Schedules
            .Include(s => s.Professional)
            .Where(s => s.ProfessionalId == professionalId)
            .OrderBy(s => s.DayOfWeek)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                ProfessionalId = s.ProfessionalId,
                ProfessionalName = s.Professional.Name + " " + s.Professional.LastName,
                DayOfWeek = s.DayOfWeek,
                DayOfWeekName = GetDayOfWeekName(s.DayOfWeek),
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                SlotDurationMinutes = s.SlotDurationMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return schedules;
    }

    public async Task<ScheduleDto?> GetScheduleByIdAsync(Guid id)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Professional)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null) return null;

        return new ScheduleDto
        {
            Id = schedule.Id,
            ProfessionalId = schedule.ProfessionalId,
            ProfessionalName = schedule.Professional.Name + " " + schedule.Professional.LastName,
            DayOfWeek = schedule.DayOfWeek,
            DayOfWeekName = GetDayOfWeekName(schedule.DayOfWeek),
            StartTime = schedule.StartTime.ToString(@"hh\:mm"),
            EndTime = schedule.EndTime.ToString(@"hh\:mm"),
            SlotDurationMinutes = schedule.SlotDurationMinutes,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto)
    {
        var schedule = new Schedule
        {
            ProfessionalId = dto.ProfessionalId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = TimeSpan.Parse(dto.StartTime),
            EndTime = TimeSpan.Parse(dto.EndTime),
            SlotDurationMinutes = dto.SlotDurationMinutes,
            IsActive = true
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        await _context.Entry(schedule).Reference(s => s.Professional).LoadAsync();

        return new ScheduleDto
        {
            Id = schedule.Id,
            ProfessionalId = schedule.ProfessionalId,
            ProfessionalName = schedule.Professional.Name + " " + schedule.Professional.LastName,
            DayOfWeek = schedule.DayOfWeek,
            DayOfWeekName = GetDayOfWeekName(schedule.DayOfWeek),
            StartTime = schedule.StartTime.ToString(@"hh\:mm"),
            EndTime = schedule.EndTime.ToString(@"hh\:mm"),
            SlotDurationMinutes = schedule.SlotDurationMinutes,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<ScheduleDto?> UpdateScheduleAsync(Guid id, UpdateScheduleDto dto)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Professional)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null) return null;

        if (!string.IsNullOrEmpty(dto.StartTime))
            schedule.StartTime = TimeSpan.Parse(dto.StartTime);

        if (!string.IsNullOrEmpty(dto.EndTime))
            schedule.EndTime = TimeSpan.Parse(dto.EndTime);

        if (dto.SlotDurationMinutes.HasValue)
            schedule.SlotDurationMinutes = dto.SlotDurationMinutes.Value;

        if (dto.IsActive.HasValue)
            schedule.IsActive = dto.IsActive.Value;

        schedule.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ScheduleDto
        {
            Id = schedule.Id,
            ProfessionalId = schedule.ProfessionalId,
            ProfessionalName = schedule.Professional.Name + " " + schedule.Professional.LastName,
            DayOfWeek = schedule.DayOfWeek,
            DayOfWeekName = GetDayOfWeekName(schedule.DayOfWeek),
            StartTime = schedule.StartTime.ToString(@"hh\:mm"),
            EndTime = schedule.EndTime.ToString(@"hh\:mm"),
            SlotDurationMinutes = schedule.SlotDurationMinutes,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<bool> DeleteScheduleAsync(Guid id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null) return false;

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ProfessionalAvailabilityDto> GetAvailabilitySlotsAsync(Guid professionalId, DateTime startDate, DateTime endDate)
    {
        var professional = await _context.Users.FindAsync(professionalId);
        if (professional == null)
            throw new InvalidOperationException("Professional not found");

        var schedules = await _context.Schedules
            .Where(s => s.ProfessionalId == professionalId && s.IsActive)
            .ToListAsync();

        var appointments = await _context.Appointments
            .Where(a => a.ProfessionalId == professionalId && 
                       a.Date >= startDate && 
                       a.Date <= endDate &&
                       a.Status != Domain.Enums.AppointmentStatus.Cancelled)
            .ToListAsync();

        var slots = new List<AvailableSlotDto>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var daySchedules = schedules.Where(s => s.DayOfWeek == dayOfWeek).ToList();

            foreach (var schedule in daySchedules)
            {
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    var slotDateTime = date.Date + currentTime;
                    var isAvailable = !appointments.Any(a => 
                        a.Date.Date == date.Date && 
                        a.Time == currentTime);

                    slots.Add(new AvailableSlotDto
                    {
                        Date = date,
                        Time = currentTime.ToString(@"hh\:mm"),
                        IsAvailable = isAvailable && slotDateTime > DateTime.Now
                    });

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
                }
            }
        }

        return new ProfessionalAvailabilityDto
        {
            ProfessionalId = professionalId,
            ProfessionalName = professional.Name + " " + professional.LastName,
            Slots = slots.OrderBy(s => s.Date).ThenBy(s => s.Time).ToList()
        };
    }

    private static string GetDayOfWeekName(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            0 => "Domingo",
            1 => "Segunda-feira",
            2 => "Terça-feira",
            3 => "Quarta-feira",
            4 => "Quinta-feira",
            5 => "Sexta-feira",
            6 => "Sábado",
            _ => "Unknown"
        };
    }
}
