using app.Domain.Entities;

namespace app.Domain.Entities;

public class ScheduleDay : BaseEntity
{
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; } = null!;
    
    // 0 = Domingo, 1 = Segunda, 2 = Terça, 3 = Quarta, 4 = Quinta, 5 = Sexta, 6 = Sábado
    public int DayOfWeek { get; set; }
    
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    // Duração padrão da consulta em minutos
    public int AppointmentDuration { get; set; }
    
    // Intervalo entre consultas em minutos
    public int IntervalBetweenAppointments { get; set; } = 0;
    
    // Pausa opcional
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
}
