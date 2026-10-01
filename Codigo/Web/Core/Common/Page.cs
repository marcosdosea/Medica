namespace Core.Common;

public record Page<T>(
    IReadOnlyList<T> Items,
    int TotalPages,
    int TotalElements,
    int PageNumber,
    int PageSize
)
{
}