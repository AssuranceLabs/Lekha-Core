using Microsoft.AspNetCore.Mvc.Filters;

namespace Makuri.Core
{
    public interface ITransactionManager
    {
        void BeginTransaction();
        void EndTransaction(ActionExecutedContext filterContext);
        void EndTransaction();
        void RollBack();
        void CloseSession();
    }
}