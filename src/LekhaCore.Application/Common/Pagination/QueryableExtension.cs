namespace LekhaCore.Application.Common.Pagination
{
    public static class QueryableExtensions
    {
        public static Task<PagedResponse<T>> ToPagedResponseAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalCount = query.Count();

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResponse<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }
    }
}
