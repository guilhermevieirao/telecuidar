using Domain.Common;

namespace Domain.Entities;

public class Schedule : BaseEntity
{
    public Guid ProfessionalId { get; set; }
    public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, etc
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public User Professional { get; set; } = null!;
}
