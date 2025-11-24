namespace app.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    void SetAuditInfo(int? userId, string? ipAddress = null, string? userAgent = null);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}