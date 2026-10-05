using LekhaCore.Application.Interfaces;
using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LekhaCore.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IWorkContext _workContext;
    private bool _writingAudit;

    public AppDbContext(DbContextOptions<AppDbContext> options, IWorkContext workContext) : base(options)
    {
        _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Menu> Menus => Set<Menu>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Notification> Notifications => Set<Notification>();

    internal bool UseSoftDelete { get; set; } = true;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (_writingAudit)
            return base.SaveChanges(acceptAllChangesOnSuccess);

        ProcessChanges();
        var auditEntries = AuditHelper.PrepareAuditEntries(
            ChangeTracker,
            CurrentActor(),
            _workContext.IpAddress,
            _workContext.CorrelationId);

        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        WriteAudits(auditEntries);
        return result;
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (_writingAudit)
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        ProcessChanges();
        var auditEntries = AuditHelper.PrepareAuditEntries(
            ChangeTracker,
            CurrentActor(),
            _workContext.IpAddress,
            _workContext.CorrelationId);

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await WriteAuditsAsync(auditEntries, cancellationToken);
        return result;
    }

    private void ProcessChanges()
    {
        var userId = CurrentActor();
        ChangeTracker.DetectChanges();
        ChangeTracker.ProcessModification(userId);

        if (UseSoftDelete)
            ChangeTracker.ProcessDeletion(userId);

        ChangeTracker.ProcessCreation(userId);
    }

    private string CurrentActor()
    {
        return string.IsNullOrWhiteSpace(_workContext.CurrentUserId)
            ? AuditActors.System
            : _workContext.CurrentUserId;
    }

    private void WriteAudits(List<AuditHelper.AuditEntry> auditEntries)
    {
        var auditLogs = AuditHelper.CreateAuditLogs(auditEntries);
        if (auditLogs.Count == 0)
            return;

        _writingAudit = true;
        try
        {
            AuditLogs.AddRange(auditLogs);
            base.SaveChanges();
        }
        finally
        {
            _writingAudit = false;
        }
    }

    private async Task WriteAuditsAsync(List<AuditHelper.AuditEntry> auditEntries, CancellationToken cancellationToken)
    {
        var auditLogs = AuditHelper.CreateAuditLogs(auditEntries);
        if (auditLogs.Count == 0)
            return;

        _writingAudit = true;
        try
        {
            await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);
            await base.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _writingAudit = false;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteFilters(modelBuilder);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()))
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<UserRole>().HasQueryFilter(assignment =>
            !assignment.IsDeleted && !assignment.User.IsDeleted && !assignment.Role.IsDeleted);

        modelBuilder.Entity<RolePermission>().HasQueryFilter(grant =>
            !grant.IsDeleted && !grant.Role.IsDeleted && !grant.Permission.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                continue;

            var method = typeof(AppDbContext)
                .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDelete
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity => !entity.IsDeleted);
    }
}
