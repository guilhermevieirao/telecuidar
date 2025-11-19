using app.Application.Common.Interfaces;

namespace app.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}