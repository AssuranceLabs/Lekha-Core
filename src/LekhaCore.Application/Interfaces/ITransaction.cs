namespace Makuri.Core
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}