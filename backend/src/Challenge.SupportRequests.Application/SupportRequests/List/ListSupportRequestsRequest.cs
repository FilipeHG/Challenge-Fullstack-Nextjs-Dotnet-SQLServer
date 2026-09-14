namespace Challenge.SupportRequests.Application.SupportRequests.List;

public sealed record ListSupportRequestsRequest
{
    public const int DefaultPage = 1;
    public const int MaximumLimit = 100;
    public int Page { get; init; } = DefaultPage;
    public int Limit { get; init; } = MaximumLimit;
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public string? Search { get; init; }
}
