using Makuri.Core;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;

namespace LekhaCore.Application.Interfaces.IRepo
{
    public interface IRepository<T> : IUnitOfWork where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<T?> GetAsync(Expression<Func<T, bool>> where);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where);

        Task<T> AddAsync(T entity);

        Task<List<T>> AddRangeAsync(List<T> entities);

        Task UpdateAsync(T entity);

        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteRangeAsync(List<T> entities);

        Task<bool> IsExistAsync(Expression<Func<T, bool>> where);

        IQueryable<T> Table { get; }

        IQueryable<T> TableNoTracking { get; }

        Task<T?> GetFirstOrLastRecordAsync(Expression<Func<T, object>> sortBy, bool isDescending = true, Expression<Func<T, bool>>? filter = null);

        Task<ICollection<TOutput>> ExecuteReaderAsync<TOutput>(string commandText, CommandType commandType, SqlParameter[]? parameters = null) where TOutput : class;
    }
}
