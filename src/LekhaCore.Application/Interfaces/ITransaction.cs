namespace LekhaCore.Application.Interfaces;

public interface ITransaction : IDisposable
{
    void Commit();

    void Rollback();
}
