using LekhaCore.Core.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LekhaCore.Infrastructure.EntityFramework
{
    public static class ChangeTrackerExtensions
    {
        public static void ProcessCreation(this ChangeTracker changeTracker, string userId)
        {
            foreach (var entry in changeTracker.Entries<IHasCreator>().Where(e => e.State == EntityState.Added))
                entry.Entity.CreatedBy = userId;

            foreach (var entry in changeTracker.Entries<ICreatedOn>().Where(e => e.State == EntityState.Added))
                entry.Entity.CreatedOn = DateTime.UtcNow;
        }

        public static void ProcessModification(this ChangeTracker changeTracker, string userId)
        {
            foreach (var entry in changeTracker.Entries<IHasModifier>().Where(e => e.State == EntityState.Modified))
                entry.Entity.ModifiedBy = userId;

            foreach (var entry in changeTracker.Entries<IModifiedOn>().Where(e => e.State == EntityState.Modified))
                entry.Entity.ModifiedOn = DateTime.UtcNow;
        }

        public static void ProcessDeletion(this ChangeTracker changeTracker, string userId)
        {
            foreach (var entry in changeTracker.Entries<IHasDeleter>().Where(e => e.State == EntityState.Deleted))
                entry.Entity.DeletedBy = userId;

            foreach (var entry in changeTracker.Entries<IDeletedOn>().Where(e => e.State == EntityState.Deleted))
                entry.Entity.DeletedOn = DateTime.UtcNow;

            foreach (var entry in changeTracker.Entries<ISoftDelete>().Where(e => e.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
            }
        }
    }
}