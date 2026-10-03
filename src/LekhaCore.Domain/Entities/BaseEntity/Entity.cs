namespace LekhaCore.Core.BaseEntity
{
    public abstract class Entity<T> : IEntity<T>
    {
        public virtual T Id { get; set; }
    }
}
