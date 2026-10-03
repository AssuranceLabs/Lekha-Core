namespace LekhaCore.Core.BaseEntity
{
    public interface IFullAudited : IHasCreator, ICreatedOn, IHasModifier, IModifiedOn, ISoftDelete, IHasDeleter, IDeletedOn
    {
    }
}
