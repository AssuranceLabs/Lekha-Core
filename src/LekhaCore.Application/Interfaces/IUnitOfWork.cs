using System.Data;

namespace Makuri.Core
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        /// <summary>
        /// Persists all changes made in this unit of work to the database.
        /// </summary>
        Task<int> CommitAsync(bool isSoftDelete = true, CancellationToken cancellationToken = default);

        ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Snapshot);
    }
}
