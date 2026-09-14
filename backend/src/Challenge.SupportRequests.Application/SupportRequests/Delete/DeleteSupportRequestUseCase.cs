using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Exceptions;

namespace Challenge.SupportRequests.Application.SupportRequests.Delete;

public sealed class DeleteSupportRequestUseCase(ISupportRequestRepository repository)
{
    public async Task ExecuteAsync(long id, CancellationToken cancellationToken)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFoundException(id);
        request.EnsureCanBeDeleted();
        if (!await repository.DeleteAsync(id, cancellationToken)) throw new ResourceNotFoundException(id);
    }
}
