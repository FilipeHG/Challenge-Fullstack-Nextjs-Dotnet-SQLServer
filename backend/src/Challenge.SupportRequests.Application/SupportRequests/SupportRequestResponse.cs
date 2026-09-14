using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;

namespace Challenge.SupportRequests.Application.SupportRequests;

public sealed record SupportRequestResponse(long Id, string Title, string Description, string Requester,
    Priority Priority, RequestStatus Status, DateTime CreatedAt, DateTime? CompletedAt)
{
    public static SupportRequestResponse From(SupportRequest request) => new(request.Id, request.Title,
        request.Description, request.Requester, request.Priority, request.Status, request.CreatedAt, request.CompletedAt);
}
