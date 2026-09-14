namespace Challenge.SupportRequests.Application.SupportRequests.Create;

public sealed record CreateSupportRequestRequest(string? Title, string? Description, string? Requester, string? Priority);
