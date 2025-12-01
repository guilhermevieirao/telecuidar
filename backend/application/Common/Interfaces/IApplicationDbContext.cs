using app.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace app.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ScheduleBlock> ScheduleBlocks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}