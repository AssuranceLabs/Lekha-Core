using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using LekhaCore.Application.Common;
using System.Reflection;

namespace LekhaCore.Infrastructure.EntityFramework;

public class Repository<T> : UnitOfWork, IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> where) => await _dbSet.FirstOrDefaultAsync(where);

    public virtual async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where) => await _dbSet.Where(where).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<List<T>> AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }

    public Task UpdateAsync(T entity)
    {
        _dbContext.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(List<T> entities)
    {
        _dbContext.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(List<T> entities)
    {
        _dbContext.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<bool> IsExistAsync(Expression<Func<T, bool>> where) => await _dbSet.AnyAsync(where);

    public virtual IQueryable<T> Table => _dbSet;

    public virtual IQueryable<T> TableNoTracking => _dbSet.AsNoTracking();

    public async Task<T?> GetFirstOrLastRecordAsync(
        Expression<Func<T, object>> sortBy,
        bool isDescending = true,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
            query = query.Where(filter);

        query = isDescending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<ICollection<TOutput>> ExecuteReaderAsync<TOutput>(
        string commandText,
        CommandType commandType,
        IReadOnlyList<QueryParameter>? parameters = null) where TOutput : class
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Command text is required.", nameof(commandText));

        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            if (_dbContext.Database.CurrentTransaction is not null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            if (parameters is not null)
            {
                foreach (var parameter in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = parameter.Name;
                    dbParameter.Value = parameter.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParameter);
                }
            }

            var results = new List<TOutput>();
            await using var reader = await command.ExecuteReaderAsync();
            var properties = typeof(TOutput)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite)
                .ToArray();

            while (await reader.ReadAsync())
            {
                var item = Activator.CreateInstance<TOutput>();
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    if (reader.IsDBNull(index))
                        continue;

                    var property = properties.FirstOrDefault(candidate =>
                        string.Equals(candidate.Name, reader.GetName(index), StringComparison.OrdinalIgnoreCase));

                    if (property is null)
                        continue;

                    var value = reader.GetValue(index);
                    var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var converted = targetType.IsEnum
                        ? Enum.ToObject(targetType, value)
                        : Convert.ChangeType(value, targetType);
                    property.SetValue(item, converted);
                }

                results.Add(item);
            }

            return results;
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }
}
