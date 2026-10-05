namespace LekhaCore.Core.BaseEntity
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
