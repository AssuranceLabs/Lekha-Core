namespace LekhaCore.Core.BaseEntity
{
    public abstract class FullAudited<T> : Entity<T>, IFullAudited
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
