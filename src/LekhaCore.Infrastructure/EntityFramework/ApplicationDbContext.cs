using LekhaCore.Core;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Core.Domain.Common.Attachment;
using LekhaCore.Core.Domain.Common.UserActivity;
using LekhaCore.Core.Domain.HR.DbEntities;
using LekhaCore.Core.Domain.Kyc.DbEntities;
using LekhaCore.Core.Domain.OutsourcedHR.DbEntities;
using LekhaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Data.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IWorkContext _workContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IWorkContext workContext) : base(options)
        {
            _workContext = workContext;
        }

        //KYC
        public DbSet<User> Users { get; set; }
      

        public virtual int Commit(bool isSoftDelete = true)
        {
            ChangeTracker.DetectChanges();
            ProcessChanges(isSoftDelete);

            var auditEntries = AuditHelper.PrepareAuditEntries(ChangeTracker, _workContext);
            var result = base.SaveChanges();

            SaveAuditLogs(auditEntries);
            return result;
        }

        public virtual async Task<int> CommitAsync(bool isSoftDelete = true, CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            ProcessChanges(isSoftDelete);

            var auditEntries = AuditHelper.PrepareAuditEntries(ChangeTracker, _workContext);
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await SaveAuditLogsAsync(auditEntries, cancellationToken).ConfigureAwait(false);
            return result;
        }

        private void ProcessChanges(bool isSoftDelete)
        {
            var userId = GetCurrentUserId();
            ChangeTracker.ProcessModification(userId);

            if (isSoftDelete)
                ChangeTracker.ProcessDeletion(userId);

            ChangeTracker.ProcessCreation(userId);
        }

        private string GetCurrentUserId() { return _workContext?.CurrentUser?.USER_ID?.ToString() ?? "SYSTEM"; }

        #region Save Audit Logs

        private void SaveAuditLogs(List<AuditHelper.AuditEntry> auditEntries)
        {
            if (auditEntries.Count == 0)
                return;

            var auditLogs = AuditHelper.CreateAuditLogs(auditEntries);
            if (auditLogs.Count == 0)
                return;

            AuditLogs.AddRange(auditLogs);
            base.SaveChanges();
        }

        private async Task SaveAuditLogsAsync(List<AuditHelper.AuditEntry> auditEntries, CancellationToken cancellationToken)
        {
            if (auditEntries.Count == 0)
                return;

            var auditLogs = AuditHelper.CreateAuditLogs(auditEntries);
            if (auditLogs.Count == 0)
                return;

            await AuditLogs.AddRangeAsync(auditLogs, cancellationToken).ConfigureAwait(false);
            await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<FiscalYear>()
            .HasIndex(x => x.FiscalYearName)
            .IsUnique();

            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }
    }
}
