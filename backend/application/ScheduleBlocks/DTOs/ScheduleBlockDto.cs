using app.Domain.Entities;
using app.Domain.Enums;

namespace app.Application.ScheduleBlocks.DTOs;

public class ScheduleBlockDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public BlockStatus Status { get; set; }
    public string? AdminJustification { get; set; }
    public int? AdminId { get; set; }
    public string? AdminName { get; set; }
    public DateTime CreatedAt { get; set; }
}