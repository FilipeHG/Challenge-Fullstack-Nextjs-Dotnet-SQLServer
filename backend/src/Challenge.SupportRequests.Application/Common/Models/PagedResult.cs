namespace Challenge.SupportRequests.Application.Common.Models;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Limit, long TotalItems)
{
    public long TotalPages => TotalItems / Limit + (TotalItems % Limit == 0 ? 0 : 1);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
