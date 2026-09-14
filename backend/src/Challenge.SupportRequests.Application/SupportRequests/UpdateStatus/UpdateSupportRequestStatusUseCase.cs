using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Exceptions;
using Challenge.SupportRequests.Domain.Enums;
using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.UpdateStatus;

public sealed class UpdateSupportRequestStatusUseCase(ISupportRequestRepository repository,
    IValidator<UpdateSupportRequestStatusRequest> validator, TimeProvider timeProvider)
{
    public async Task<SupportRequestResponse> ExecuteAsync(long id, UpdateSupportRequestStatusRequest input, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var request = await repository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFoundException(id);
        request.ChangeStatus(Enum.Parse<RequestStatus>(input.Status!, true), timeProvider.GetUtcNow().UtcDateTime);
        if (!await repository.UpdateStatusAsync(request, cancellationToken)) throw new ResourceNotFoundException(id);
        return SupportRequestResponse.From(request);
    }
}
