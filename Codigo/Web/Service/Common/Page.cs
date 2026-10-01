using Core.Common;

namespace Service.Common;

public static class PaginationHandler
{
    public static Page<T> ToPage<T>(
        this IQueryable<T> query,
        int pageNumber = 1,
        int pageSize = 10)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalElements = query.Count();

        var totalPages = pageSize > 0
            ? (int)Math.Ceiling(totalElements / (double)pageSize)
            : 0;

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Page<T>(
            items,
            totalPages,
            totalElements,
            pageNumber,
            pageSize
        );
    }
}