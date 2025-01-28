using System.Collections.Generic;
using MyFirstApi.Pagination;

namespace MyFirstApi.Models
{
    public interface IPaginatedReadRepository<TId, T> : IReadRepository<TId, T>, IDateQueryable<T>
    {
        PaginatedResult<T> FindPaginated(int pageNumber, int pageSize);
        PaginatedResult<T> FindPaginatedByDate(DateOnly date, int pageNumber, int pageSize);
    }
}