using LekhaCore.Application.Common.Exceptions;
using LekhaCore.Application.Interfaces;
using LekhaCore.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LekhaCore.Infrastructure.EntityFramework;

public class UnitOfWork : IUnitOfWork
{
    protected readonly AppDbContext DbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Commit()
    {
        DbContext.SaveChanges();
    }

    public async Task<int> CommitAsync(bool isSoftDelete = true, CancellationToken cancellationToken = default)
    {
        DbContext.UseSoftDelete = isSoftDelete;

        try
        {
            return await DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            throw new UniqueConstraintException("A unique constraint was violated.", exception);
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException sql && (sql.Number is 2601 or 2627);

    public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var current = DbContext.Database.CurrentTransaction;
        var transaction = current ?? DbContext.Database.BeginTransaction(isolationLevel);
        return new DbTransaction(transaction);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
