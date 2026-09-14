using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Exceptions;

namespace Challenge.SupportRequests.Application.SupportRequests.GetById;

public sealed class GetSupportRequestByIdUseCase(ISupportRequestRepository repository)
{
    public async Task<SupportRequestResponse> ExecuteAsync(long id, CancellationToken cancellationToken)
        => SupportRequestResponse.From(await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException(id));
}
