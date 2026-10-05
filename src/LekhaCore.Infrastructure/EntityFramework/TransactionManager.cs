using LekhaCore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Infrastructure.EntityFramework;

public class TransactionManager : ITransactionManager
{
    private IUnitOfWork? _unitOfWork;
    private ITransaction? _transaction;
    private bool _isOpen;
    private readonly ILogger _logger;

    public TransactionManager(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = loggerFactory.CreateLogger("ActionTransactionHelper");
    }

    public void BeginTransaction()
    {
        _transaction = _unitOfWork?.BeginTransaction();
        _isOpen = true;
    }

    public void EndTransaction(Exception? exception)
    {
        try
        {
            if (_transaction is null || _unitOfWork is null)
                throw new NotSupportedException("Transaction has not been started.");

            if (exception is null)
            {
                if (_isOpen)
                {
                    _unitOfWork.Commit();
                    _transaction.Commit();
                    _isOpen = false;
                }
            }
            else
            {
                _transaction.Rollback();
                _isOpen = false;
            }
        }
        catch (Exception completionFailure)
        {
            _logger.LogError(completionFailure, "Failed to complete the database transaction.");
            throw;
        }
    }

    public void EndTransaction()
    {
        if (_isOpen && _unitOfWork is not null && _transaction is not null)
        {
            _unitOfWork.Commit();
            _transaction.Commit();
            _isOpen = false;
            return;
        }

        try
        {
            _transaction?.Rollback();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to roll back the database transaction.");
        }
    }

    public void RollBack()
    {
        try
        {
            _isOpen = false;
            _transaction?.Rollback();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to roll back the database transaction.");
        }
    }

    public void CloseSession()
    {
        _transaction?.Dispose();
        _transaction = null;
        _unitOfWork?.Dispose();
        _unitOfWork = null;
    }
}
