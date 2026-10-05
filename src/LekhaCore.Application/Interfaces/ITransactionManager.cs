namespace LekhaCore.Application.Interfaces;

public interface ITransactionManager
{
    void BeginTransaction();

    void EndTransaction(Exception? exception);

    void EndTransaction();

    void RollBack();

    void CloseSession();
}
