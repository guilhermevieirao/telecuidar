using app.Domain.Entities;

namespace app.Domain.Entities;

public class Schedule : BaseEntity
{
    public int ProfessionalId { get; set; }
    public User Professional { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Navegação
    public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
}
