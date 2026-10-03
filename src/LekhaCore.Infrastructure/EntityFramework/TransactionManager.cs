using LekhaCore.Core;
using LekhaCore.Core.ServiceResult;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Data.EntityFramework
{
    public class TransactionManager : ITransactionManager
    {
        private IUnitOfWork _uow;
        private ITransaction _tx;
        private bool isOpen = false;

        private readonly ILogger _log;

        public TransactionManager(IUnitOfWork uow, ILoggerFactory log)
        {
            _uow = uow;
            _log = log.CreateLogger("ActionTransactionHelper");
        }

        public void BeginTransaction()
        {
            _tx = _uow.BeginTransaction();
            isOpen = true;
        }

        public void EndTransaction(ActionExecutedContext filterContext)
        {
            try
            {
                if (_tx == null) throw new NotSupportedException();
                if (filterContext.Exception == null)
                {
                    if (isOpen)
                    {
                        dynamic d = filterContext.Result;
                        if (d != null && d.Value is ServiceResult)
                        {

                            var res = (ServiceResult)d.Value;
                            if (!res.Status)
                            {
                                _tx.Rollback();
                                return;
                            }
                        }
                        _uow.Commit();
                        _tx.Commit();
                        isOpen = false;
                    }

                }
                else
                {
                    _tx.Rollback();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new AggregateException(filterContext.Exception, ex);
            }
        }

        public void EndTransaction()
        {
            if (isOpen)
            {
                _uow.Commit();
                _tx.Commit();
                isOpen = false;
            }
            try
            {
                _tx.Rollback();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }

        public void RollBack()
        {
            try
            {
                isOpen = false;
                _tx.Rollback();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }

        public void CloseSession()
        {
            if (_tx != null)
            {
                _tx.Dispose();
                _tx = null;
            }

            if (_uow != null)
            {
                _uow.Dispose();
                _uow = null;
            }
        }
    }
}