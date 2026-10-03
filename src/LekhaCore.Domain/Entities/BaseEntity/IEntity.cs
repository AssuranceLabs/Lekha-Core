namespace LekhaCore.Core.BaseEntity
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
