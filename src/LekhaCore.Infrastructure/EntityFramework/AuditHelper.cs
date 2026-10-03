using LekhaCore.Core;
using LekhaCore.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using System.Text.Json;

namespace LekhaCore.Data.EntityFramework
{
    public static class AuditHelper
    {
        private static readonly HashSet<string> IgnoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "CreatedBy",
            "CreatedOn",
            "ModifiedBy",
            "ModifiedOn",
            "DeletedBy",
            "DeletedOn"
        };

        public static List<AuditEntry> PrepareAuditEntries(ChangeTracker changeTracker, IWorkContext workContext)
        {
            var auditEntries = new List<AuditEntry>();

            var entries = changeTracker.Entries<IAuditableEntity>()
                .Where(entry =>
                    entry.Entity is not AuditLog &&
                    (entry.State == EntityState.Added ||
                     entry.State == EntityState.Modified ||
                     entry.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
            {
                var auditEntry = new AuditEntry
                {
                    EntityEntry = entry,
                    TableName = GetTableName(entry),
                    Type = GetAuditType(entry),
                    SubType = GetAuditSubType(entry.Metadata.ClrType),
                    UserId = GetCurrentUserId(workContext),
                    DateTime = DateTime.UtcNow
                };

                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.IsPrimaryKey())
                        continue;

                    if (IgnoredProperties.Contains(property.Metadata.Name))
                        continue;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                            break;

                        case EntityState.Modified:
                            if (!property.IsModified)
                                continue;

                            if (Equals(property.OriginalValue, property.CurrentValue))
                                continue;

                            auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                            auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                            auditEntry.AffectedColumns.Add(property.Metadata.Name);
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                            break;
                    }
                }

                if (entry.State == EntityState.Modified && auditEntry.AffectedColumns.Count == 0)
                    continue;

                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }

        public static List<AuditLog> CreateAuditLogs(IEnumerable<AuditEntry> auditEntries)
        {
            var auditLogs = new List<AuditLog>();

            foreach (var auditEntry in auditEntries)
            {
                var primaryKey = GetPrimaryKeyValue(auditEntry.EntityEntry);

                var auditLog = new AuditLog
                {
                    UserId = auditEntry.UserId,
                    Type = auditEntry.Type,
                    SubType = auditEntry.SubType,
                    TableName = auditEntry.TableName,
                    DateTime = auditEntry.DateTime,
                    PrimaryKey = primaryKey,
                    OldValues = auditEntry.OldValues.Count > 0
                        ? JsonSerializer.Serialize(auditEntry.OldValues)
                        : null,
                    NewValues = auditEntry.NewValues.Count > 0
                        ? JsonSerializer.Serialize(auditEntry.NewValues)
                        : null,
                    AffectedColumns = auditEntry.AffectedColumns.Count > 0
                        ? JsonSerializer.Serialize(auditEntry.AffectedColumns)
                        : null
                };

                auditLogs.Add(auditLog);
            }

            return auditLogs;
        }

        private static string GetTableName(EntityEntry<IAuditableEntity> entry)
        {
            var tableName = entry.Metadata.GetTableName();
            var schema = entry.Metadata.GetSchema();

            if (!string.IsNullOrWhiteSpace(schema) && !string.IsNullOrWhiteSpace(tableName))
                return $"{schema}.{tableName}";

            return tableName ?? entry.Metadata.ClrType.Name;
        }

        private static string GetAuditSubType(Type entityType)
        {
            var attribute = entityType.GetCustomAttribute<AuditSubTypeAttribute>();
            return attribute?.Name ?? string.Empty;
        }

        private static string GetCurrentUserId(IWorkContext workContext)
        {
            return workContext?.CurrentUser?.USER_ID?.ToString() ?? "SYSTEM";
        }

        private static string GetAuditType(EntityEntry<IAuditableEntity> entry)
        {
            if (entry.State == EntityState.Modified && IsSoftDelete(entry))
                return "Delete";

            return entry.State switch
            {
                EntityState.Added => "Create",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };
        }

        private static bool IsSoftDelete(EntityEntry<IAuditableEntity> entry)
        {
            var property = entry.Properties.FirstOrDefault(x =>
                x.Metadata.Name.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase));

            if (property == null || !property.IsModified)
                return false;

            return Equals(property.OriginalValue, false) &&
                   Equals(property.CurrentValue, true);
        }

        private static string GetPrimaryKeyValue(EntityEntry<IAuditableEntity> entry)
        {
            var primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey == null)
                return string.Empty;

            if (primaryKey.Properties.Count == 1)
            {
                var property = primaryKey.Properties[0];
                var value = entry.Property(property.Name).CurrentValue;
                return value?.ToString() ?? string.Empty;
            }

            var keyValues = new Dictionary<string, object?>();

            foreach (var property in primaryKey.Properties)
                keyValues[property.Name] = entry.Property(property.Name).CurrentValue;

            return JsonSerializer.Serialize(keyValues);
        }

        public sealed class AuditEntry
        {
            public EntityEntry<IAuditableEntity> EntityEntry { get; set; } = null!;
            public string TableName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string? SubType { get; set; }
            public string UserId { get; set; } = string.Empty;
            public DateTime DateTime { get; set; }
            public Dictionary<string, object?> OldValues { get; } = new();
            public Dictionary<string, object?> NewValues { get; } = new();
            public List<string> AffectedColumns { get; } = new();
        }
    }
}