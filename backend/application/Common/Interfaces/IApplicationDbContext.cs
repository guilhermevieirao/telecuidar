using app.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace app.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ScheduleBlock> ScheduleBlocks { get; }
    DbSet<SpecialtyField> SpecialtyFields { get; }
    DbSet<AppointmentFieldValue> AppointmentFieldValues { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<Specialty> Specialties { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}