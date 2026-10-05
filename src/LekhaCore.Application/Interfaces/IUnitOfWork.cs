using System.Data;

namespace LekhaCore.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    void Commit();

    Task<int> CommitAsync(bool isSoftDelete = true, CancellationToken cancellationToken = default);

    ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}
