using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;

namespace Challenge.SupportRequests.Infrastructure.Persistence;

internal sealed class SupportRequestPersistenceModel
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Requester { get; set; } = "";
    public Priority Priority { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public SupportRequest ToDomain() => SupportRequest.Rehydrate(Id, Title, Description, Requester,
        Priority, Status, CreatedAt, CompletedAt);
}
