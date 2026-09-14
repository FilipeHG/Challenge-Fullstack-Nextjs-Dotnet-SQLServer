using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;

namespace Challenge.SupportRequests.Application.Abstractions.Persistence;

public sealed record SupportRequestFilter(RequestStatus? Status, Priority? Priority, string? Search, int Page, int Limit);
public sealed record SupportRequestPage(IReadOnlyList<SupportRequest> Items, long TotalItems);

public interface ISupportRequestRepository
{
    Task<long> CreateAsync(SupportRequest request, CancellationToken cancellationToken);
    Task<SupportRequest?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<SupportRequestPage> ListAsync(SupportRequestFilter filter, CancellationToken cancellationToken);
    Task<bool> UpdatePriorityAsync(SupportRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateStatusAsync(SupportRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
