using LekhaCore.Core;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LekhaCore.Data.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Commit()
        {
            _dbContext.Commit();
        }

        /// <summary>
        /// Commits all tracked changes to the database.
        /// </summary>
        public async Task<int> CommitAsync(bool isSoftDelete = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.CommitAsync(isSoftDelete, cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException dbEx)
            {
                // Wrap EF-specific exception with context, preserve inner exception
                throw new Exception("An error occurred while committing changes to the database.", dbEx);
            }
            catch
            {
                // Re-throw everything else, preserving stack trace
                throw;
            }
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new DbTransaction(_dbContext.Database.CurrentTransaction ?? _dbContext.Database.BeginTransaction(isolationLevel));
        }

        /// <summary>
        /// Disposes the DbContext when this UoW is disposed.
        /// </summary>
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
