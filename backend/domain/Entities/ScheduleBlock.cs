using app.Domain.Enums;

namespace app.Domain.Entities;

public enum BlockStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}

public class ScheduleBlock : BaseEntity
{
    public int ProfessionalId { get; set; }
    public User Professional { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public BlockStatus Status { get; set; } = BlockStatus.Pending;
    public string? AdminJustification { get; set; }
    public int? AdminId { get; set; }
    public User? Admin { get; set; }
}