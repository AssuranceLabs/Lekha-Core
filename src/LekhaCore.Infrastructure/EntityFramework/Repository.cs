using LekhaCore.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace LekhaCore.Data.EntityFramework
{
    public class Repository<T> : UnitOfWork, IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetAsync(Expression<Func<T, bool>> where) => await _dbSet.FirstOrDefaultAsync(where);

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

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
            _dbContext.Update(entity); // safer than Attach + Modified
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

        public async Task<T?> GetFirstOrLastRecordAsync(Expression<Func<T, object>> sortBy, bool isDescending = true, Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);
            query = isDescending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);
            return await query.FirstOrDefaultAsync();
        }

        protected virtual async Task<T> ExecuteReaderAsync<T>(Func<DbDataReader, T> mapEntities, string exec, SqlParameter[]? parameters = null)
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = exec;

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            //NOTE Removed this because data didnot bind in multiset object if 1st result set is null.
            //TODO But this could result in error.
            //if (!reader.HasRows)
            //    while (await reader.NextResultAsync()) { } 

            T data = mapEntities(reader);
            return data;
        }

        public async Task<ICollection<TOutput>> ExecuteReaderAsync<TOutput>(string commandText, CommandType commandType, SqlParameter[]? parameters = null) where TOutput : class
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
                while (await reader.NextResultAsync()) { }

            var mapper = new DataReaderMapper<TOutput>();
            return mapper.MapToList(reader);
        }
    }
}
