namespace LekhaCore.Core.BaseEntity
{
    public interface IHasDeleter
    {
        string? DeletedBy { get; set; }
    }
}
